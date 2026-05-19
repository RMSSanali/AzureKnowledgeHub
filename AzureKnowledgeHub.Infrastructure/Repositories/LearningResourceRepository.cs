using AzureKnowledgeHub.Application.Interfaces;
using AzureKnowledgeHub.Domain.Entities;
using AzureKnowledgeHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AzureKnowledgeHub.Infrastructure.Repositories;

public class LearningResourceRepository : ILearningResourceRepository
{
    private readonly AzureKnowledgeHubDbContext _dbContext;

    public LearningResourceRepository(AzureKnowledgeHubDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<LearningResource>> GetAllAsync()
    {
        return _dbContext.LearningResources
            .AsNoTracking()
            .Include(resource => resource.Category)
            .Include(resource => resource.ResourceTags)
            .ThenInclude(resourceTag => resourceTag.Tag)
            .OrderByDescending(resource => resource.CreatedAt)
            .ToListAsync();
    }

    public Task<LearningResource?> GetByIdAsync(int id)
    {
        return _dbContext.LearningResources
            .Include(resource => resource.Category)
            .Include(resource => resource.ResourceTags)
            .ThenInclude(resourceTag => resourceTag.Tag)
            .FirstOrDefaultAsync(resource => resource.LearningResourceId == id);
    }

    public async Task<LearningResource> CreateAsync(LearningResource resource, List<string> tags)
    {
        await EnsureCategoryExistsAsync(resource.CategoryId);
        await EnsureUserExistsIfProvidedAsync(resource.CreatedByUserId);

        var tagEntities = await GetOrCreateTagsAsync(tags);
        foreach (var tag in tagEntities)
        {
            resource.ResourceTags.Add(new ResourceTag
            {
                LearningResource = resource,
                Tag = tag
            });
        }

        _dbContext.LearningResources.Add(resource);
        await _dbContext.SaveChangesAsync();
        _dbContext.ChangeTracker.Clear();

        return await GetByIdAsync(resource.LearningResourceId)
            ?? resource;
    }

    public async Task<LearningResource?> UpdateAsync(LearningResource resource, List<string> tags)
    {
        if (!await ExistsAsync(resource.LearningResourceId))
        {
            return null;
        }

        await EnsureCategoryExistsAsync(resource.CategoryId);

        var existingTags = await _dbContext.ResourceTags
            .Where(resourceTag => resourceTag.LearningResourceId == resource.LearningResourceId)
            .ToListAsync();

        _dbContext.ResourceTags.RemoveRange(existingTags);

        var tagEntities = await GetOrCreateTagsAsync(tags);
        foreach (var tag in tagEntities)
        {
            _dbContext.ResourceTags.Add(new ResourceTag
            {
                LearningResourceId = resource.LearningResourceId,
                Tag = tag
            });
        }

        await _dbContext.SaveChangesAsync();
        _dbContext.ChangeTracker.Clear();

        return await GetByIdAsync(resource.LearningResourceId);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var resource = await _dbContext.LearningResources
            .Include(learningResource => learningResource.ResourceTags)
            .FirstOrDefaultAsync(learningResource => learningResource.LearningResourceId == id);

        if (resource is null)
        {
            return false;
        }

        _dbContext.ResourceTags.RemoveRange(resource.ResourceTags);
        _dbContext.LearningResources.Remove(resource);
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public Task<bool> ExistsAsync(int id)
    {
        return _dbContext.LearningResources
            .AnyAsync(resource => resource.LearningResourceId == id);
    }

    private async Task EnsureCategoryExistsAsync(int categoryId)
    {
        var categoryExists = await _dbContext.Categories
            .AnyAsync(category => category.CategoryId == categoryId);

        if (!categoryExists)
        {
            throw new ArgumentException("The selected category does not exist.");
        }
    }

    private async Task EnsureUserExistsIfProvidedAsync(int? userId)
    {
        if (!userId.HasValue)
        {
            return;
        }

        var userExists = await _dbContext.Users
            .AnyAsync(user => user.UserId == userId.Value);

        if (!userExists)
        {
            throw new ArgumentException("The selected user does not exist.");
        }
    }

    private async Task<List<Tag>> GetOrCreateTagsAsync(List<string> tags)
    {
        var tagNames = NormalizeTagNames(tags);
        if (tagNames.Count == 0)
        {
            return new List<Tag>();
        }

        var existingTags = await _dbContext.Tags
            .Where(tag => tagNames.Contains(tag.Name))
            .ToListAsync();

        var newTags = tagNames
            .Where(tagName => existingTags.All(tag =>
                !string.Equals(tag.Name, tagName, StringComparison.OrdinalIgnoreCase)))
            .Select(tagName => new Tag { Name = tagName })
            .ToList();

        if (newTags.Count > 0)
        {
            _dbContext.Tags.AddRange(newTags);
            existingTags.AddRange(newTags);
        }

        return existingTags;
    }

    private static List<string> NormalizeTagNames(List<string> tags)
    {
        return tags
            .Where(tag => !string.IsNullOrWhiteSpace(tag))
            .Select(tag => tag.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}

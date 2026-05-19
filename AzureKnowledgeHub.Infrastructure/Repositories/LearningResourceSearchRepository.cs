using AzureKnowledgeHub.Application.DTOs;
using AzureKnowledgeHub.Application.Interfaces;
using AzureKnowledgeHub.Application.Search;
using AzureKnowledgeHub.Domain.Entities;
using AzureKnowledgeHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AzureKnowledgeHub.Infrastructure.Repositories;

public class LearningResourceSearchRepository : ILearningResourceSearchRepository
{
    private readonly AzureKnowledgeHubDbContext _dbContext;

    public LearningResourceSearchRepository(AzureKnowledgeHubDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<LearningResourceSearchRepositoryResult> SearchAsync(LearningResourceSearchRequestDto request)
    {
        var query = _dbContext.LearningResources
            .AsNoTracking()
            .Include(resource => resource.Category)
            .Include(resource => resource.ResourceTags)
            .ThenInclude(resourceTag => resourceTag.Tag)
            .AsQueryable();

        query = ApplyKeywordSearch(query, request.Query);
        query = ApplyFilters(query, request);

        var totalCount = await query.CountAsync();
        var resources = await query
            .OrderByDescending(resource => resource.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return new LearningResourceSearchRepositoryResult
        {
            Resources = resources,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    private static IQueryable<LearningResource> ApplyKeywordSearch(
        IQueryable<LearningResource> query,
        string? searchQuery)
    {
        if (string.IsNullOrWhiteSpace(searchQuery))
        {
            return query;
        }

        return query.Where(resource =>
            resource.Title.Contains(searchQuery) ||
            resource.Summary.Contains(searchQuery) ||
            resource.Content.Contains(searchQuery) ||
            resource.CertificationPath.Contains(searchQuery) ||
            (resource.Category != null && resource.Category.Name.Contains(searchQuery)) ||
            resource.ResourceTags.Any(resourceTag => resourceTag.Tag.Name.Contains(searchQuery)));
    }

    private static IQueryable<LearningResource> ApplyFilters(
        IQueryable<LearningResource> query,
        LearningResourceSearchRequestDto request)
    {
        if (request.CategoryId.HasValue)
        {
            query = query.Where(resource => resource.CategoryId == request.CategoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.CategoryName))
        {
            query = query.Where(resource =>
                resource.Category != null &&
                resource.Category.Name.Contains(request.CategoryName));
        }

        if (!string.IsNullOrWhiteSpace(request.CertificationPath))
        {
            query = query.Where(resource => resource.CertificationPath == request.CertificationPath);
        }

        if (!string.IsNullOrWhiteSpace(request.DifficultyLevel))
        {
            query = query.Where(resource => resource.DifficultyLevel == request.DifficultyLevel);
        }

        if (!string.IsNullOrWhiteSpace(request.Tag))
        {
            query = query.Where(resource =>
                resource.ResourceTags.Any(resourceTag => resourceTag.Tag.Name.Contains(request.Tag)));
        }

        if (request.IsPublished.HasValue)
        {
            query = query.Where(resource => resource.IsPublished == request.IsPublished.Value);
        }

        return query;
    }
}

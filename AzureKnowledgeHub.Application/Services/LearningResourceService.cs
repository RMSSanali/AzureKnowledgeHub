using AzureKnowledgeHub.Application.DTOs;
using AzureKnowledgeHub.Application.Interfaces;
using AzureKnowledgeHub.Domain.Entities;

namespace AzureKnowledgeHub.Application.Services;

public class LearningResourceService : ILearningResourceService
{
    private readonly ILearningResourceRepository _repository;

    public LearningResourceService(ILearningResourceRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<LearningResourceDto>> GetAllAsync()
    {
        var resources = await _repository.GetAllAsync();

        return resources.Select(MapToDto).ToList();
    }

    public async Task<LearningResourceDto?> GetByIdAsync(int id)
    {
        var resource = await _repository.GetByIdAsync(id);

        return resource is null ? null : MapToDto(resource);
    }

    public async Task<LearningResourceDto> CreateAsync(CreateLearningResourceDto dto)
    {
        ValidateCreateDto(dto);

        var resource = new LearningResource
        {
            Title = dto.Title.Trim(),
            Summary = dto.Summary.Trim(),
            Content = dto.Content.Trim(),
            CertificationPath = dto.CertificationPath.Trim(),
            DifficultyLevel = dto.DifficultyLevel.Trim(),
            CategoryId = dto.CategoryId,
            CreatedByUserId = dto.CreatedByUserId,
            CreatedAt = DateTime.UtcNow,
            IsPublished = dto.IsPublished
        };

        var createdResource = await _repository.CreateAsync(resource, dto.Tags);

        return MapToDto(createdResource);
    }

    public async Task<LearningResourceDto?> UpdateAsync(int id, UpdateLearningResourceDto dto)
    {
        ValidateUpdateDto(dto);

        var resource = await _repository.GetByIdAsync(id);
        if (resource is null)
        {
            return null;
        }

        resource.Title = dto.Title.Trim();
        resource.Summary = dto.Summary.Trim();
        resource.Content = dto.Content.Trim();
        resource.CertificationPath = dto.CertificationPath.Trim();
        resource.DifficultyLevel = dto.DifficultyLevel.Trim();
        resource.CategoryId = dto.CategoryId;
        resource.IsPublished = dto.IsPublished;
        resource.UpdatedAt = DateTime.UtcNow;

        var updatedResource = await _repository.UpdateAsync(resource, dto.Tags);

        return updatedResource is null ? null : MapToDto(updatedResource);
    }

    public Task<bool> DeleteAsync(int id)
    {
        return _repository.DeleteAsync(id);
    }

    private static LearningResourceDto MapToDto(LearningResource resource)
    {
        return new LearningResourceDto
        {
            LearningResourceId = resource.LearningResourceId,
            Title = resource.Title,
            Summary = resource.Summary,
            Content = resource.Content,
            CertificationPath = resource.CertificationPath,
            DifficultyLevel = resource.DifficultyLevel,
            CategoryId = resource.CategoryId,
            CategoryName = resource.Category?.Name ?? string.Empty,
            CreatedByUserId = resource.CreatedByUserId,
            CreatedAt = resource.CreatedAt,
            UpdatedAt = resource.UpdatedAt,
            IsPublished = resource.IsPublished,
            Tags = resource.ResourceTags
                .Where(resourceTag => resourceTag.Tag is not null)
                .Select(resourceTag => resourceTag.Tag.Name)
                .OrderBy(tag => tag)
                .ToList()
        };
    }

    private static void ValidateCreateDto(CreateLearningResourceDto dto)
    {
        ValidateResourceFields(
            dto.Title,
            dto.Summary,
            dto.Content,
            dto.CertificationPath,
            dto.DifficultyLevel,
            dto.CategoryId);
    }

    private static void ValidateUpdateDto(UpdateLearningResourceDto dto)
    {
        ValidateResourceFields(
            dto.Title,
            dto.Summary,
            dto.Content,
            dto.CertificationPath,
            dto.DifficultyLevel,
            dto.CategoryId);
    }

    private static void ValidateResourceFields(
        string title,
        string summary,
        string content,
        string certificationPath,
        string difficultyLevel,
        int categoryId)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title is required.");
        }

        if (string.IsNullOrWhiteSpace(summary))
        {
            throw new ArgumentException("Summary is required.");
        }

        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ArgumentException("Content is required.");
        }

        if (string.IsNullOrWhiteSpace(certificationPath))
        {
            throw new ArgumentException("Certification path is required.");
        }

        if (string.IsNullOrWhiteSpace(difficultyLevel))
        {
            throw new ArgumentException("Difficulty level is required.");
        }

        if (categoryId <= 0)
        {
            throw new ArgumentException("A valid category is required.");
        }
    }
}

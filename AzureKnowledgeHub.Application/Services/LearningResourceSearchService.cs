using AzureKnowledgeHub.Application.DTOs;
using AzureKnowledgeHub.Application.Interfaces;
using AzureKnowledgeHub.Domain.Entities;

namespace AzureKnowledgeHub.Application.Services;

public class LearningResourceSearchService : ILearningResourceSearchService
{
    private const int MaxPageSize = 100;
    private readonly ILearningResourceSearchRepository _searchRepository;

    public LearningResourceSearchService(ILearningResourceSearchRepository searchRepository)
    {
        _searchRepository = searchRepository;
    }

    public async Task<LearningResourceSearchResponseDto> SearchAsync(LearningResourceSearchRequestDto request)
    {
        ValidatePagination(request.PageNumber, request.PageSize);

        var normalizedRequest = NormalizeRequest(request);
        var searchResult = await _searchRepository.SearchAsync(normalizedRequest);

        return new LearningResourceSearchResponseDto
        {
            Items = searchResult.Resources.Select(MapToDto).ToList(),
            TotalCount = searchResult.TotalCount,
            PageNumber = searchResult.PageNumber,
            PageSize = searchResult.PageSize,
            TotalPages = CalculateTotalPages(searchResult.TotalCount, searchResult.PageSize)
        };
    }

    private static LearningResourceSearchRequestDto NormalizeRequest(LearningResourceSearchRequestDto request)
    {
        return new LearningResourceSearchRequestDto
        {
            Query = NormalizeText(request.Query),
            CategoryId = request.CategoryId,
            CategoryName = NormalizeText(request.CategoryName),
            CertificationPath = NormalizeText(request.CertificationPath),
            DifficultyLevel = NormalizeText(request.DifficultyLevel),
            Tag = NormalizeText(request.Tag),
            IsPublished = request.IsPublished,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    private static string? NormalizeText(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private static void ValidatePagination(int pageNumber, int pageSize)
    {
        if (pageNumber < 1)
        {
            throw new ArgumentException("PageNumber must be 1 or greater.");
        }

        if (pageSize < 1)
        {
            throw new ArgumentException("PageSize must be 1 or greater.");
        }

        if (pageSize > MaxPageSize)
        {
            throw new ArgumentException($"PageSize cannot be greater than {MaxPageSize}.");
        }
    }

    private static int CalculateTotalPages(int totalCount, int pageSize)
    {
        if (totalCount == 0)
        {
            return 0;
        }

        return (int)Math.Ceiling(totalCount / (double)pageSize);
    }

    private static LearningResourceSearchResultDto MapToDto(LearningResource resource)
    {
        return new LearningResourceSearchResultDto
        {
            LearningResourceId = resource.LearningResourceId,
            Title = resource.Title,
            Summary = resource.Summary,
            CertificationPath = resource.CertificationPath,
            DifficultyLevel = resource.DifficultyLevel,
            CategoryName = resource.Category?.Name ?? string.Empty,
            Tags = resource.ResourceTags
                .Where(resourceTag => resourceTag.Tag is not null)
                .Select(resourceTag => resourceTag.Tag.Name)
                .OrderBy(tag => tag)
                .ToList(),
            CreatedAt = resource.CreatedAt,
            UpdatedAt = resource.UpdatedAt,
            IsPublished = resource.IsPublished
        };
    }
}

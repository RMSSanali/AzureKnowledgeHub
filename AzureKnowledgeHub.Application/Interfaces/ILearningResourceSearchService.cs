using AzureKnowledgeHub.Application.DTOs;

namespace AzureKnowledgeHub.Application.Interfaces;

public interface ILearningResourceSearchService
{
    Task<LearningResourceSearchResponseDto> SearchAsync(LearningResourceSearchRequestDto request);
}

using AzureKnowledgeHub.Application.DTOs;
using AzureKnowledgeHub.Application.Search;

namespace AzureKnowledgeHub.Application.Interfaces;

public interface ILearningResourceSearchRepository
{
    Task<LearningResourceSearchRepositoryResult> SearchAsync(LearningResourceSearchRequestDto request);
}

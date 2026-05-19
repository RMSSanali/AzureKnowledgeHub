using AzureKnowledgeHub.Domain.Entities;

namespace AzureKnowledgeHub.Application.Interfaces;

public interface ILearningResourceRepository
{
    Task<List<LearningResource>> GetAllAsync();

    Task<LearningResource?> GetByIdAsync(int id);

    Task<LearningResource> CreateAsync(LearningResource resource, List<string> tags);

    Task<LearningResource?> UpdateAsync(LearningResource resource, List<string> tags);

    Task<bool> DeleteAsync(int id);

    Task<bool> ExistsAsync(int id);
}

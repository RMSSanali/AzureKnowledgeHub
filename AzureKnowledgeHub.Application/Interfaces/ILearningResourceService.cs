using AzureKnowledgeHub.Application.DTOs;

namespace AzureKnowledgeHub.Application.Interfaces;

public interface ILearningResourceService
{
    Task<List<LearningResourceDto>> GetAllAsync();

    Task<LearningResourceDto?> GetByIdAsync(int id);

    Task<LearningResourceDto> CreateAsync(CreateLearningResourceDto dto);

    Task<LearningResourceDto?> UpdateAsync(int id, UpdateLearningResourceDto dto);

    Task<bool> DeleteAsync(int id);
}

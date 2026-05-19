using AzureKnowledgeHub.Domain.Entities;

namespace AzureKnowledgeHub.Application.Search;

public class LearningResourceSearchRepositoryResult
{
    public List<LearningResource> Resources { get; set; } = new();

    public int TotalCount { get; set; }

    public int PageNumber { get; set; }

    public int PageSize { get; set; }
}

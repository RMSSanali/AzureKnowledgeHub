namespace AzureKnowledgeHub.Application.DTOs;

public class LearningResourceSearchRequestDto
{
    public string? Query { get; set; }

    public int? CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public string? CertificationPath { get; set; }

    public string? DifficultyLevel { get; set; }

    public string? Tag { get; set; }

    public bool? IsPublished { get; set; }

    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}

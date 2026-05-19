namespace AzureKnowledgeHub.Application.DTOs;

public class LearningResourceDto
{
    public int LearningResourceId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Summary { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public string CertificationPath { get; set; } = string.Empty;

    public string DifficultyLevel { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public int? CreatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsPublished { get; set; }

    public List<string> Tags { get; set; } = new();
}

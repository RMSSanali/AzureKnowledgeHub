namespace AzureKnowledgeHub.Application.DTOs;

public class UpdateLearningResourceDto
{
    public string Title { get; set; } = string.Empty;

    public string Summary { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public string CertificationPath { get; set; } = string.Empty;

    public string DifficultyLevel { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    public bool IsPublished { get; set; }

    public List<string> Tags { get; set; } = new();
}

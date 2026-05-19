namespace AzureKnowledgeHub.Domain.Entities;

public class LearningResource
{
    public int LearningResourceId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Summary { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public string CertificationPath { get; set; } = string.Empty; // Example: AZ-900, AZ-104

    public string DifficultyLevel { get; set; } = string.Empty; // Example: Beginner, Intermediate, Advanced

    public int CategoryId { get; set; }

    public int? CreatedByUserId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public bool IsPublished { get; set; } = true;

    // Navigation properties
    public Category? Category { get; set; }

    public User? CreatedByUser { get; set; }

    public ICollection<ResourceTag> ResourceTags { get; set; } = new List<ResourceTag>();

    public ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
}
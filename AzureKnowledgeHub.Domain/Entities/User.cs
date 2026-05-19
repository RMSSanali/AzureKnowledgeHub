namespace AzureKnowledgeHub.Domain.Entities;

public class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string Role { get; set; } = "Learner";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<LearningResource> CreatedResources { get; set; } = new List<LearningResource>();
}

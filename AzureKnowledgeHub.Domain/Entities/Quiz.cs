namespace AzureKnowledgeHub.Domain.Entities;

public class Quiz
{
    public int QuizId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int? LearningResourceId { get; set; }

    public LearningResource? LearningResource { get; set; }

    public ICollection<Question> Questions { get; set; } = new List<Question>();
}
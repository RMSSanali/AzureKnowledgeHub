namespace AzureKnowledgeHub.Domain.Entities;

public class Question
{
    public int QuestionId { get; set; }

    public int QuizId { get; set; }

    public Quiz Quiz { get; set; } = null!;

    public string QuestionText { get; set; } = string.Empty;

    public string Explanation { get; set; } = string.Empty;

    public ICollection<QuestionOption> Options { get; set; } = new List<QuestionOption>();
}
namespace AzureKnowledgeHub.Domain.Entities;

public class QuestionOption
{
    public int QuestionOptionId { get; set; }

    public int QuestionId { get; set; }

    public Question Question { get; set; } = null!;

    public string OptionText { get; set; } = string.Empty;

    public bool IsCorrect { get; set; }
}
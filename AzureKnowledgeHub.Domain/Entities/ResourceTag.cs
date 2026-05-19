namespace AzureKnowledgeHub.Domain.Entities;

public class ResourceTag
{
    public int LearningResourceId { get; set; }

    public LearningResource LearningResource { get; set; } = null!;

    public int TagId { get; set; }

    public Tag Tag { get; set; } = null!;
}
namespace AzureKnowledgeHub.Domain.Entities;

public class Tag
{
    public int TagId { get; set; }

    public string Name { get; set; } = string.Empty;

    public ICollection<ResourceTag> ResourceTags { get; set; } = new List<ResourceTag>();
}
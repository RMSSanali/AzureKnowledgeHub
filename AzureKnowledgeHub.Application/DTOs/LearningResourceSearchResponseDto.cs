namespace AzureKnowledgeHub.Application.DTOs;

public class LearningResourceSearchResponseDto
{
    public List<LearningResourceSearchResultDto> Items { get; set; } = new();

    public int TotalCount { get; set; }

    public int PageNumber { get; set; }

    public int PageSize { get; set; }

    public int TotalPages { get; set; }
}

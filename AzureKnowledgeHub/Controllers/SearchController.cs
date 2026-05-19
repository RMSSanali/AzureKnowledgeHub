using AzureKnowledgeHub.Application.DTOs;
using AzureKnowledgeHub.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AzureKnowledgeHub.Controllers;

[ApiController]
[Route("api/search")]
public class SearchController : ControllerBase
{
    private readonly ILearningResourceSearchService _searchService;

    public SearchController(ILearningResourceSearchService searchService)
    {
        _searchService = searchService;
    }

    [HttpGet("resources")]
    [ProducesResponseType(typeof(LearningResourceSearchResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LearningResourceSearchResponseDto>> SearchResources(
        [FromQuery] LearningResourceSearchRequestDto request)
    {
        try
        {
            var result = await _searchService.SearchAsync(request);

            return Ok(result);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(exception.Message);
        }
    }
}

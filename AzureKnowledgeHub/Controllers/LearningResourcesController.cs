using AzureKnowledgeHub.Application.DTOs;
using AzureKnowledgeHub.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AzureKnowledgeHub.Controllers;

[ApiController]
[Route("api/learningresources")]
public class LearningResourcesController : ControllerBase
{
    private readonly ILearningResourceService _learningResourceService;

    public LearningResourcesController(ILearningResourceService learningResourceService)
    {
        _learningResourceService = learningResourceService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<LearningResourceDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<LearningResourceDto>>> GetAll()
    {
        var resources = await _learningResourceService.GetAllAsync();

        return Ok(resources);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(LearningResourceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LearningResourceDto>> GetById(int id)
    {
        var resource = await _learningResourceService.GetByIdAsync(id);
        if (resource is null)
        {
            return NotFound();
        }

        return Ok(resource);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(LearningResourceDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<LearningResourceDto>> Create(CreateLearningResourceDto dto)
    {
        try
        {
            var resource = await _learningResourceService.CreateAsync(dto);

            return CreatedAtAction(nameof(GetById), new { id = resource.LearningResourceId }, resource);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(exception.Message);
        }
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(LearningResourceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LearningResourceDto>> Update(int id, UpdateLearningResourceDto dto)
    {
        try
        {
            var resource = await _learningResourceService.UpdateAsync(id, dto);
            if (resource is null)
            {
                return NotFound();
            }

            return Ok(resource);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(exception.Message);
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _learningResourceService.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}

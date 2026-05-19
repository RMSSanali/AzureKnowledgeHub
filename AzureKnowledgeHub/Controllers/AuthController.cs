using AzureKnowledgeHub.Application.DTOs;
using AzureKnowledgeHub.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AzureKnowledgeHub.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterUserDto dto)
    {
        try
        {
            var result = await _authService.RegisterAsync(dto);

            return CreatedAtAction(nameof(Register), result);
        }
        catch (ArgumentException exception)
        {
            return BadRequest(exception.Message);
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(exception.Message);
        }
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginUserDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        if (result is null)
        {
            return Unauthorized("Invalid username, email, or password.");
        }

        return Ok(result);
    }
}

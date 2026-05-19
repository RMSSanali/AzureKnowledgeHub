using AzureKnowledgeHub.Application.DTOs;

namespace AzureKnowledgeHub.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterUserDto dto);

    Task<AuthResponseDto?> LoginAsync(LoginUserDto dto);
}

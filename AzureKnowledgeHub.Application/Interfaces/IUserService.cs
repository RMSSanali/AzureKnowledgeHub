using AzureKnowledgeHub.Application.DTOs;

namespace AzureKnowledgeHub.Application.Interfaces;

public interface IUserService
{
    Task<UserProfileDto?> GetProfileAsync(int userId);

    Task<UserProfileDto?> UpdateProfileAsync(int userId, UpdateProfileDto dto);

    Task<List<AdminUserDto>> GetAllUsersAsync();

    Task<AdminUserDto> CreateAdminAsync(CreateAdminUserDto dto);
}

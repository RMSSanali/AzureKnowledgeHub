using AzureKnowledgeHub.Application.DTOs;
using AzureKnowledgeHub.Application.Interfaces;
using AzureKnowledgeHub.Domain.Entities;

namespace AzureKnowledgeHub.Application.Services;

public class UserService : IUserService
{
    private const string AdminRole = "Admin";
    private readonly IPasswordService _passwordService;
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository, IPasswordService passwordService)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
    }

    public async Task<UserProfileDto?> GetProfileAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        return user is null ? null : MapToProfileDto(user);
    }

    public async Task<UserProfileDto?> UpdateProfileAsync(int userId, UpdateProfileDto dto)
    {
        ValidateProfileDto(dto);

        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null)
        {
            return null;
        }

        var email = dto.Email.Trim();
        var existingUserWithEmail = await _userRepository.GetByEmailAsync(email);
        if (existingUserWithEmail is not null && existingUserWithEmail.UserId != userId)
        {
            throw new InvalidOperationException("Email is already registered.");
        }

        user.Email = email;
        user.DisplayName = dto.DisplayName.Trim();

        var updatedUser = await _userRepository.UpdateAsync(user);

        return MapToProfileDto(updatedUser);
    }

    public async Task<List<AdminUserDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();

        return users.Select(MapToAdminDto).ToList();
    }

    public async Task<AdminUserDto> CreateAdminAsync(CreateAdminUserDto dto)
    {
        ValidateCreateAdminDto(dto);

        var username = dto.Username.Trim();
        var email = dto.Email.Trim();

        if (await _userRepository.UsernameExistsAsync(username))
        {
            throw new InvalidOperationException("Username is already registered.");
        }

        if (await _userRepository.EmailExistsAsync(email))
        {
            throw new InvalidOperationException("Email is already registered.");
        }

        var user = new User
        {
            Username = username,
            Email = email,
            DisplayName = dto.DisplayName.Trim(),
            Role = AdminRole,
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash = _passwordService.HashPassword(user, dto.Password);

        var createdUser = await _userRepository.CreateAsync(user);

        return MapToAdminDto(createdUser);
    }

    private static UserProfileDto MapToProfileDto(User user)
    {
        return new UserProfileDto
        {
            UserId = user.UserId,
            Username = user.Username,
            Email = user.Email,
            DisplayName = user.DisplayName,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };
    }

    private static AdminUserDto MapToAdminDto(User user)
    {
        return new AdminUserDto
        {
            UserId = user.UserId,
            Username = user.Username,
            Email = user.Email,
            DisplayName = user.DisplayName,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };
    }

    private static void ValidateProfileDto(UpdateProfileDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email))
        {
            throw new ArgumentException("Email is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.DisplayName))
        {
            throw new ArgumentException("Display name is required.");
        }
    }

    private static void ValidateCreateAdminDto(CreateAdminUserDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Username))
        {
            throw new ArgumentException("Username is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.Email))
        {
            throw new ArgumentException("Email is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.DisplayName))
        {
            throw new ArgumentException("Display name is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 6)
        {
            throw new ArgumentException("Password must be at least 6 characters long.");
        }
    }
}

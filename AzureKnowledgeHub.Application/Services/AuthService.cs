using AzureKnowledgeHub.Application.DTOs;
using AzureKnowledgeHub.Application.Interfaces;
using AzureKnowledgeHub.Domain.Entities;

namespace AzureKnowledgeHub.Application.Services;

public class AuthService : IAuthService
{
    private const string LearnerRole = "Learner";
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IPasswordService _passwordService;
    private readonly IUserRepository _userRepository;

    public AuthService(
        IUserRepository userRepository,
        IPasswordService passwordService,
        IJwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterUserDto dto)
    {
        ValidateRegisterDto(dto);

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
            Role = LearnerRole,
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash = _passwordService.HashPassword(user, dto.Password);

        var createdUser = await _userRepository.CreateAsync(user);

        return CreateAuthResponse(createdUser);
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginUserDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.UsernameOrEmail) || string.IsNullOrWhiteSpace(dto.Password))
        {
            return null;
        }

        var usernameOrEmail = dto.UsernameOrEmail.Trim();
        var user = await _userRepository.GetByEmailAsync(usernameOrEmail)
            ?? await _userRepository.GetByUsernameAsync(usernameOrEmail);

        if (user is null || !_passwordService.VerifyPassword(user, dto.Password))
        {
            return null;
        }

        return CreateAuthResponse(user);
    }

    private AuthResponseDto CreateAuthResponse(User user)
    {
        return new AuthResponseDto
        {
            Token = _jwtTokenService.CreateToken(user),
            UserId = user.UserId,
            Username = user.Username,
            Email = user.Email,
            DisplayName = user.DisplayName,
            Role = user.Role
        };
    }

    private static void ValidateRegisterDto(RegisterUserDto dto)
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

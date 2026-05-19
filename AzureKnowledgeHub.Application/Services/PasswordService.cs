using AzureKnowledgeHub.Application.Interfaces;
using AzureKnowledgeHub.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace AzureKnowledgeHub.Application.Services;

public class PasswordService : IPasswordService
{
    private readonly PasswordHasher<User> _passwordHasher = new();

    public string HashPassword(User user, string password)
    {
        return _passwordHasher.HashPassword(user, password);
    }

    public bool VerifyPassword(User user, string password)
    {
        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

        return result == PasswordVerificationResult.Success ||
            result == PasswordVerificationResult.SuccessRehashNeeded;
    }
}

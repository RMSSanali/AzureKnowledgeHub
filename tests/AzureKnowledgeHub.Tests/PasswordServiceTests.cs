using AzureKnowledgeHub.Application.Services;
using AzureKnowledgeHub.Domain.Entities;
using FluentAssertions;

namespace AzureKnowledgeHub.Tests;

public class PasswordServiceTests
{
    [Fact]
    public void HashPassword_DoesNotReturnPlainPassword_And_VerifiesCorrectPassword()
    {
        var service = new PasswordService();
        var user = new User { Username = "learner", Email = "learner@example.com" };
        const string password = "Learner123!";

        var hash = service.HashPassword(user, password);
        user.PasswordHash = hash;

        hash.Should().NotBe(password);
        service.VerifyPassword(user, password).Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_WithWrongPassword_ReturnsFalse()
    {
        var service = new PasswordService();
        var user = new User { Username = "learner", Email = "learner@example.com" };
        user.PasswordHash = service.HashPassword(user, "Learner123!");

        var result = service.VerifyPassword(user, "WrongPassword123!");

        result.Should().BeFalse();
    }
}

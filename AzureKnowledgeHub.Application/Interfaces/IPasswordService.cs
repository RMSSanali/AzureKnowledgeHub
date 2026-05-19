using AzureKnowledgeHub.Domain.Entities;

namespace AzureKnowledgeHub.Application.Interfaces;

public interface IPasswordService
{
    string HashPassword(User user, string password);

    bool VerifyPassword(User user, string password);
}

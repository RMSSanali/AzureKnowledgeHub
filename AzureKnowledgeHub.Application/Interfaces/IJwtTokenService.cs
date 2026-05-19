using AzureKnowledgeHub.Domain.Entities;

namespace AzureKnowledgeHub.Application.Interfaces;

public interface IJwtTokenService
{
    string CreateToken(User user);
}

using AzureKnowledgeHub.Domain.Entities;

namespace AzureKnowledgeHub.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);

    Task<User?> GetByUsernameAsync(string username);

    Task<User?> GetByEmailAsync(string email);

    Task<List<User>> GetAllAsync();

    Task<User> CreateAsync(User user);

    Task<User> UpdateAsync(User user);

    Task<bool> UsernameExistsAsync(string username);

    Task<bool> EmailExistsAsync(string email);

    Task<int> CountAdminsAsync();
}

using AzureKnowledgeHub.Application.Interfaces;
using AzureKnowledgeHub.Domain.Entities;
using AzureKnowledgeHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AzureKnowledgeHub.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AzureKnowledgeHubDbContext _dbContext;

    public UserRepository(AzureKnowledgeHubDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<User?> GetByIdAsync(int id)
    {
        return _dbContext.Users.FirstOrDefaultAsync(user => user.UserId == id);
    }

    public Task<User?> GetByUsernameAsync(string username)
    {
        return _dbContext.Users.FirstOrDefaultAsync(user => user.Username.ToLower() == username.ToLower());
    }

    public Task<User?> GetByEmailAsync(string email)
    {
        return _dbContext.Users.FirstOrDefaultAsync(user => user.Email.ToLower() == email.ToLower());
    }

    public Task<List<User>> GetAllAsync()
    {
        return _dbContext.Users
            .AsNoTracking()
            .OrderBy(user => user.Role)
            .ThenBy(user => user.Username)
            .ToListAsync();
    }

    public async Task<User> CreateAsync(User user)
    {
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        return user;
    }

    public async Task<User> UpdateAsync(User user)
    {
        await _dbContext.SaveChangesAsync();

        return user;
    }

    public Task<bool> UsernameExistsAsync(string username)
    {
        return _dbContext.Users.AnyAsync(user => user.Username.ToLower() == username.ToLower());
    }

    public Task<bool> EmailExistsAsync(string email)
    {
        return _dbContext.Users.AnyAsync(user => user.Email.ToLower() == email.ToLower());
    }

    public Task<int> CountAdminsAsync()
    {
        return _dbContext.Users.CountAsync(user => user.Role == "Admin");
    }
}

using AzureKnowledgeHub.Application.Interfaces;
using AzureKnowledgeHub.Infrastructure.Data;
using AzureKnowledgeHub.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AzureKnowledgeHub.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<AzureKnowledgeHubDbContext>(options =>
            options.UseSqlServer(connectionString));
        services.AddScoped<ILearningResourceRepository, LearningResourceRepository>();
        services.AddScoped<ILearningResourceSearchRepository, LearningResourceSearchRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}

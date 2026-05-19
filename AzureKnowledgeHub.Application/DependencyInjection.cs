using AzureKnowledgeHub.Application.Interfaces;
using AzureKnowledgeHub.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AzureKnowledgeHub.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ILearningResourceService, LearningResourceService>();
        services.AddScoped<ILearningResourceSearchService, LearningResourceSearchService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IPasswordService, PasswordService>();

        return services;
    }
}

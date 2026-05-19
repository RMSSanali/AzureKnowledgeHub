using AzureKnowledgeHub.Application.Interfaces;
using AzureKnowledgeHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AzureKnowledgeHub.Infrastructure.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AzureKnowledgeHubDbContext>();
        var passwordService = scope.ServiceProvider.GetRequiredService<IPasswordService>();

        await SeedCategoriesAsync(dbContext);
        await SeedTagsAsync(dbContext);
        await SeedLearningResourcesAsync(dbContext);
        await SeedDevelopmentAdminAsync(dbContext, passwordService);
    }

    private static async Task SeedCategoriesAsync(AzureKnowledgeHubDbContext dbContext)
    {
        var categories = new[]
        {
            new Category { Name = "Fundamentals", Description = "Core Azure concepts and cloud foundations" },
            new Category { Name = "Security", Description = "Azure security, identity, and governance topics" },
            new Category { Name = "Networking", Description = "Azure networking services and connectivity" },
            new Category { Name = "Storage", Description = "Azure storage services and data options" },
            new Category { Name = "Compute", Description = "Azure compute and application hosting services" }
        };

        var existingCategoryNames = await dbContext.Categories
            .Select(category => category.Name)
            .ToListAsync();

        var categoriesToAdd = categories
            .Where(category => !existingCategoryNames.Contains(category.Name, StringComparer.OrdinalIgnoreCase))
            .ToList();

        if (categoriesToAdd.Count == 0)
        {
            return;
        }

        dbContext.Categories.AddRange(categoriesToAdd);
        await dbContext.SaveChangesAsync();
    }

    private static async Task SeedTagsAsync(AzureKnowledgeHubDbContext dbContext)
    {
        var tagNames = new[]
        {
            "Azure",
            "AZ-900",
            "AZ-104",
            "Cloud",
            "Storage",
            "Security",
            "Networking",
            "Compute",
            "Identity",
            "Monitoring"
        };

        var existingTagNames = await dbContext.Tags
            .Select(tag => tag.Name)
            .ToListAsync();

        var tagsToAdd = tagNames
            .Where(tagName => !existingTagNames.Contains(tagName, StringComparer.OrdinalIgnoreCase))
            .Select(tagName => new Tag { Name = tagName })
            .ToList();

        if (tagsToAdd.Count == 0)
        {
            return;
        }

        dbContext.Tags.AddRange(tagsToAdd);
        await dbContext.SaveChangesAsync();
    }

    private static async Task SeedLearningResourcesAsync(AzureKnowledgeHubDbContext dbContext)
    {
        var categories = await dbContext.Categories.ToListAsync();
        var tags = await dbContext.Tags.ToListAsync();
        var existingResourceTitles = await dbContext.LearningResources
            .Select(resource => resource.Title)
            .ToListAsync();

        var demoResources = BuildDemoResources(categories);
        var resourcesToAdd = demoResources
            .Where(resource => !existingResourceTitles.Contains(resource.Resource.Title, StringComparer.OrdinalIgnoreCase))
            .ToList();

        foreach (var demoResource in resourcesToAdd)
        {
            AddTagsToResource(demoResource.Resource, demoResource.TagNames, tags);
            dbContext.LearningResources.Add(demoResource.Resource);
        }

        if (resourcesToAdd.Count > 0)
        {
            await dbContext.SaveChangesAsync();
        }

        await EnsureResourceTagLinksAsync(dbContext, demoResources, tags);
    }

    private static List<DemoLearningResource> BuildDemoResources(List<Category> categories)
    {
        return new List<DemoLearningResource>
        {
            CreateDemoResource(
                categories,
                "Introduction to Azure Cloud Concepts",
                "Introduces cloud computing, Azure global infrastructure, and core service models.",
                "This resource explains cloud concepts such as high availability, scalability, elasticity, and consumption-based pricing. It connects these ideas to common Azure services used in certification learning.",
                "AZ-900",
                "Beginner",
                "Fundamentals",
                new DateTime(2026, 1, 10, 8, 0, 0, DateTimeKind.Utc),
                "Azure", "AZ-900", "Cloud"),
            CreateDemoResource(
                categories,
                "Azure Storage Accounts Overview",
                "Covers storage accounts, blobs, files, queues, and tables for foundational Azure learning.",
                "This resource describes how Azure Storage accounts organize data services and how storage redundancy, access tiers, and secure access support cloud workloads.",
                "AZ-900",
                "Beginner",
                "Storage",
                new DateTime(2026, 1, 11, 8, 0, 0, DateTimeKind.Utc),
                "Azure", "AZ-900", "Storage", "Cloud"),
            CreateDemoResource(
                categories,
                "Azure Virtual Networks Basics",
                "Explains virtual networks, subnets, private IP addressing, and network security groups.",
                "This resource introduces Azure Virtual Network as the foundation for private connectivity between Azure resources. It also explains basic subnet design and traffic control.",
                "AZ-104",
                "Intermediate",
                "Networking",
                new DateTime(2026, 1, 12, 8, 0, 0, DateTimeKind.Utc),
                "Azure", "AZ-104", "Networking"),
            CreateDemoResource(
                categories,
                "Azure Identity and Access Management",
                "Introduces identity, access management, role-based access control, and Microsoft Entra ID.",
                "This resource explains how Azure uses identity as a security boundary. It covers users, groups, roles, and least privilege access for administration scenarios.",
                "AZ-104",
                "Intermediate",
                "Security",
                new DateTime(2026, 1, 13, 8, 0, 0, DateTimeKind.Utc),
                "Azure", "AZ-104", "Security", "Identity"),
            CreateDemoResource(
                categories,
                "Azure Monitoring with Application Insights",
                "Shows how monitoring supports reliability, diagnostics, and application visibility in Azure.",
                "This resource describes metrics, logs, alerts, and Application Insights telemetry. It explains how monitoring helps detect failures and understand application behavior.",
                "AZ-104",
                "Intermediate",
                "Fundamentals",
                new DateTime(2026, 1, 14, 8, 0, 0, DateTimeKind.Utc),
                "Azure", "AZ-104", "Monitoring"),
            CreateDemoResource(
                categories,
                "Azure App Service Hosting",
                "Explains how Azure App Service hosts web applications without managing servers.",
                "This resource introduces App Service plans, deployment slots, scaling options, and managed platform hosting for web APIs and applications.",
                "AZ-104",
                "Intermediate",
                "Compute",
                new DateTime(2026, 1, 15, 8, 0, 0, DateTimeKind.Utc),
                "Azure", "AZ-104", "Compute", "Cloud"),
            CreateDemoResource(
                categories,
                "Azure SQL Database Basics",
                "Introduces Azure SQL Database as a managed relational database service.",
                "This resource explains managed database features such as backups, scalability, connectivity, and security options for Azure SQL Database.",
                "AZ-900",
                "Beginner",
                "Storage",
                new DateTime(2026, 1, 16, 8, 0, 0, DateTimeKind.Utc),
                "Azure", "AZ-900", "Storage"),
            CreateDemoResource(
                categories,
                "Shared Responsibility Model in Azure",
                "Explains how responsibility for security and operations is shared between Microsoft and the customer.",
                "This resource compares responsibility across SaaS, PaaS, and IaaS models and explains why the model matters for Azure security planning.",
                "AZ-900",
                "Beginner",
                "Security",
                new DateTime(2026, 1, 17, 8, 0, 0, DateTimeKind.Utc),
                "Azure", "AZ-900", "Security", "Cloud"),
            CreateDemoResource(
                categories,
                "Azure Compute Services Overview",
                "Compares virtual machines, containers, functions, and app hosting options in Azure.",
                "This resource introduces common Azure compute services and explains when each option is suitable for certification scenarios and cloud solution design.",
                "AZ-900",
                "Beginner",
                "Compute",
                new DateTime(2026, 1, 18, 8, 0, 0, DateTimeKind.Utc),
                "Azure", "AZ-900", "Compute"),
            CreateDemoResource(
                categories,
                "Preparing for AZ-900 Exam",
                "Provides a structured overview of study areas for the AZ-900 certification exam.",
                "This resource summarizes key AZ-900 exam domains, including cloud concepts, Azure services, security, pricing, governance, and support.",
                "AZ-900",
                "Beginner",
                "Fundamentals",
                new DateTime(2026, 1, 19, 8, 0, 0, DateTimeKind.Utc),
                "Azure", "AZ-900", "Cloud")
        };
    }

    private static DemoLearningResource CreateDemoResource(
        List<Category> categories,
        string title,
        string summary,
        string content,
        string certificationPath,
        string difficultyLevel,
        string categoryName,
        DateTime createdAt,
        params string[] tagNames)
    {
        var category = categories.First(category =>
            string.Equals(category.Name, categoryName, StringComparison.OrdinalIgnoreCase));

        return new DemoLearningResource(
            new LearningResource
            {
                Title = title,
                Summary = summary,
                Content = content,
                CertificationPath = certificationPath,
                DifficultyLevel = difficultyLevel,
                CategoryId = category.CategoryId,
                CreatedByUserId = null,
                CreatedAt = createdAt,
                IsPublished = true
            },
            tagNames.ToList());
    }

    private static void AddTagsToResource(
        LearningResource resource,
        List<string> tagNames,
        List<Tag> tags)
    {
        foreach (var tagName in tagNames)
        {
            var tag = tags.First(tag => string.Equals(tag.Name, tagName, StringComparison.OrdinalIgnoreCase));
            resource.ResourceTags.Add(new ResourceTag
            {
                LearningResource = resource,
                Tag = tag
            });
        }
    }

    private static async Task EnsureResourceTagLinksAsync(
        AzureKnowledgeHubDbContext dbContext,
        List<DemoLearningResource> demoResources,
        List<Tag> tags)
    {
        var resourceTitles = demoResources.Select(resource => resource.Resource.Title).ToList();
        var existingResources = await dbContext.LearningResources
            .Include(resource => resource.ResourceTags)
            .Where(resource => resourceTitles.Contains(resource.Title))
            .ToListAsync();

        foreach (var demoResource in demoResources)
        {
            var resource = existingResources.First(existingResource =>
                string.Equals(existingResource.Title, demoResource.Resource.Title, StringComparison.OrdinalIgnoreCase));

            foreach (var tagName in demoResource.TagNames)
            {
                var tag = tags.First(tag => string.Equals(tag.Name, tagName, StringComparison.OrdinalIgnoreCase));
                var linkExists = resource.ResourceTags.Any(resourceTag => resourceTag.TagId == tag.TagId);

                if (!linkExists)
                {
                    dbContext.ResourceTags.Add(new ResourceTag
                    {
                        LearningResourceId = resource.LearningResourceId,
                        TagId = tag.TagId
                    });
                }
            }
        }

        await dbContext.SaveChangesAsync();
    }

    private static async Task SeedDevelopmentAdminAsync(
        AzureKnowledgeHubDbContext dbContext,
        IPasswordService passwordService)
    {
        const string username = "admin";
        const string email = "admin@azureknowledgehub.local";

        var adminExists = await dbContext.Users
            .AnyAsync(user => user.Username == username || user.Email == email);

        if (adminExists)
        {
            return;
        }

        var admin = new User
        {
            Username = username,
            Email = email,
            DisplayName = "Admin User",
            Role = "Admin",
            CreatedAt = DateTime.UtcNow
        };

        admin.PasswordHash = passwordService.HashPassword(admin, "Admin123!");

        dbContext.Users.Add(admin);
        await dbContext.SaveChangesAsync();
    }

    private sealed record DemoLearningResource(LearningResource Resource, List<string> TagNames);
}

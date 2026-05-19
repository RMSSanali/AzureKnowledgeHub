using AzureKnowledgeHub.Application.DTOs;
using AzureKnowledgeHub.Application.Interfaces;
using AzureKnowledgeHub.Application.Services;
using AzureKnowledgeHub.Domain.Entities;
using FluentAssertions;
using Moq;

namespace AzureKnowledgeHub.Tests;

public class LearningResourceServiceTests
{
    [Fact]
    public async Task CreateAsync_WhenTitleIsMissing_ThrowsArgumentException()
    {
        var repository = new Mock<ILearningResourceRepository>(MockBehavior.Strict);
        var service = new LearningResourceService(repository.Object);

        var act = () => service.CreateAsync(new CreateLearningResourceDto
        {
            Title = " ",
            Summary = "Summary",
            Content = "Content",
            CertificationPath = "AZ-900",
            DifficultyLevel = "Beginner",
            CategoryId = 1
        });

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Title is required.");
    }

    [Fact]
    public async Task GetByIdAsync_WhenResourceDoesNotExist_ReturnsNull()
    {
        var repository = new Mock<ILearningResourceRepository>();
        repository.Setup(repo => repo.GetByIdAsync(42)).ReturnsAsync((LearningResource?)null);
        var service = new LearningResourceService(repository.Object);

        var result = await service.GetByIdAsync(42);

        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_WhenContentIsMissing_ThrowsArgumentException()
    {
        var repository = new Mock<ILearningResourceRepository>(MockBehavior.Strict);
        var service = new LearningResourceService(repository.Object);

        var act = () => service.UpdateAsync(1, new UpdateLearningResourceDto
        {
            Title = "Title",
            Summary = "Summary",
            Content = "",
            CertificationPath = "AZ-900",
            DifficultyLevel = "Beginner",
            CategoryId = 1,
            IsPublished = true
        });

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Content is required.");
    }

    [Fact]
    public async Task GetByIdAsync_WhenResourceExists_MapsEntityToDto()
    {
        var repository = new Mock<ILearningResourceRepository>();
        repository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(CreateResource());
        var service = new LearningResourceService(repository.Object);

        var result = await service.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.Title.Should().Be("Azure Virtual Networks Basics");
        result.CategoryName.Should().Be("Networking");
        result.Tags.Should().Equal("AZ-104", "Networking");
    }

    private static LearningResource CreateResource()
    {
        var resource = new LearningResource
        {
            LearningResourceId = 1,
            Title = "Azure Virtual Networks Basics",
            Summary = "Networking summary",
            Content = "Networking content",
            CertificationPath = "AZ-104",
            DifficultyLevel = "Intermediate",
            CategoryId = 2,
            Category = new Category { CategoryId = 2, Name = "Networking" },
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            IsPublished = true
        };

        resource.ResourceTags = new List<ResourceTag>
        {
            new() { LearningResource = resource, Tag = new Tag { TagId = 2, Name = "Networking" } },
            new() { LearningResource = resource, Tag = new Tag { TagId = 1, Name = "AZ-104" } }
        };

        return resource;
    }
}

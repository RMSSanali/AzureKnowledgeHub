using AzureKnowledgeHub.Application.DTOs;
using AzureKnowledgeHub.Application.Interfaces;
using AzureKnowledgeHub.Application.Search;
using AzureKnowledgeHub.Application.Services;
using AzureKnowledgeHub.Domain.Entities;
using FluentAssertions;
using Moq;

namespace AzureKnowledgeHub.Tests;

public class LearningResourceSearchServiceTests
{
    [Fact]
    public async Task SearchAsync_WhenPageNumberIsInvalid_ThrowsArgumentException()
    {
        var repository = new Mock<ILearningResourceSearchRepository>(MockBehavior.Strict);
        var service = new LearningResourceSearchService(repository.Object);

        var act = () => service.SearchAsync(new LearningResourceSearchRequestDto { PageNumber = 0 });

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("PageNumber must be 1 or greater.");
    }

    [Fact]
    public async Task SearchAsync_WhenPageSizeIsTooLarge_ThrowsArgumentException()
    {
        var repository = new Mock<ILearningResourceSearchRepository>(MockBehavior.Strict);
        var service = new LearningResourceSearchService(repository.Object);

        var act = () => service.SearchAsync(new LearningResourceSearchRequestDto { PageSize = 101 });

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("PageSize cannot be greater than 100.");
    }

    [Fact]
    public async Task SearchAsync_WithValidRequest_NormalizesInputAndCallsRepository()
    {
        var repository = new Mock<ILearningResourceSearchRepository>();
        repository
            .Setup(repo => repo.SearchAsync(It.Is<LearningResourceSearchRequestDto>(request =>
                request.Query == "storage" &&
                request.CategoryName == null &&
                request.Tag == "Azure" &&
                request.PageNumber == 2 &&
                request.PageSize == 5)))
            .ReturnsAsync(new LearningResourceSearchRepositoryResult
            {
                Resources = new List<LearningResource>
                {
                    CreateResource()
                },
                TotalCount = 6,
                PageNumber = 2,
                PageSize = 5
            });

        var service = new LearningResourceSearchService(repository.Object);

        var result = await service.SearchAsync(new LearningResourceSearchRequestDto
        {
            Query = " storage ",
            CategoryName = "   ",
            Tag = " Azure ",
            PageNumber = 2,
            PageSize = 5
        });

        result.TotalCount.Should().Be(6);
        result.TotalPages.Should().Be(2);
        result.Items.Should().ContainSingle();
        result.Items[0].CategoryName.Should().Be("Storage");
        result.Items[0].Tags.Should().Equal("AZ-900", "Azure");
        repository.VerifyAll();
    }

    [Fact]
    public async Task SearchAsync_WithDefaultRequest_UsesDefaultPagination()
    {
        var repository = new Mock<ILearningResourceSearchRepository>();
        repository
            .Setup(repo => repo.SearchAsync(It.Is<LearningResourceSearchRequestDto>(request =>
                request.Query == null &&
                request.PageNumber == 1 &&
                request.PageSize == 10)))
            .ReturnsAsync(new LearningResourceSearchRepositoryResult
            {
                Resources = new List<LearningResource>(),
                TotalCount = 0,
                PageNumber = 1,
                PageSize = 10
            });

        var service = new LearningResourceSearchService(repository.Object);

        var result = await service.SearchAsync(new LearningResourceSearchRequestDto());

        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.TotalPages.Should().Be(0);
        repository.VerifyAll();
    }

    private static LearningResource CreateResource()
    {
        var resource = new LearningResource
        {
            LearningResourceId = 1,
            Title = "Azure Storage Accounts Overview",
            Summary = "Storage summary",
            Content = "Storage content",
            CertificationPath = "AZ-900",
            DifficultyLevel = "Beginner",
            CategoryId = 1,
            Category = new Category { CategoryId = 1, Name = "Storage" },
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            IsPublished = true
        };

        resource.ResourceTags = new List<ResourceTag>
        {
            new() { LearningResource = resource, Tag = new Tag { TagId = 2, Name = "Azure" } },
            new() { LearningResource = resource, Tag = new Tag { TagId = 1, Name = "AZ-900" } }
        };

        return resource;
    }
}

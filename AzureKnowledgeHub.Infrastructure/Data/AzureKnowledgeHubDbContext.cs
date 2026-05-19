using AzureKnowledgeHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AzureKnowledgeHub.Infrastructure.Data;

public class AzureKnowledgeHubDbContext : DbContext
{
    public AzureKnowledgeHubDbContext(DbContextOptions<AzureKnowledgeHubDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<LearningResource> LearningResources => Set<LearningResource>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<ResourceTag> ResourceTags => Set<ResourceTag>();
    public DbSet<Quiz> Quizzes => Set<Quiz>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<QuestionOption> QuestionOptions => Set<QuestionOption>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(user => user.Username)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(user => user.Email)
            .IsUnique();

        modelBuilder.Entity<ResourceTag>()
            .HasKey(rt => new { rt.LearningResourceId, rt.TagId });

        modelBuilder.Entity<ResourceTag>()
            .HasOne(rt => rt.LearningResource)
            .WithMany(lr => lr.ResourceTags)
            .HasForeignKey(rt => rt.LearningResourceId);

        modelBuilder.Entity<ResourceTag>()
            .HasOne(rt => rt.Tag)
            .WithMany(t => t.ResourceTags)
            .HasForeignKey(rt => rt.TagId);

        modelBuilder.Entity<LearningResource>()
            .HasOne(lr => lr.Category)
            .WithMany(c => c.LearningResources)
            .HasForeignKey(lr => lr.CategoryId);

        modelBuilder.Entity<LearningResource>()
            .HasOne(lr => lr.CreatedByUser)
            .WithMany(u => u.CreatedResources)
            .HasForeignKey(lr => lr.CreatedByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Quiz>()
            .HasOne(q => q.LearningResource)
            .WithMany(lr => lr.Quizzes)
            .HasForeignKey(q => q.LearningResourceId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Question>()
            .HasOne(q => q.Quiz)
            .WithMany(qz => qz.Questions)
            .HasForeignKey(q => q.QuizId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<QuestionOption>()
            .HasOne(o => o.Question)
            .WithMany(q => q.Options)
            .HasForeignKey(o => o.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

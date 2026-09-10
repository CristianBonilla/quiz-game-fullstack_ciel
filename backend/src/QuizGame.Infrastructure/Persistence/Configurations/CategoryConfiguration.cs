using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuizGame.Domain.Categories;
using QuizGame.Domain.ValueObjects;
using QuizGame.Infrastructure.Persistence.Seed;

namespace QuizGame.Infrastructure.Persistence.Configurations;

public sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");
        builder.HasKey(category => category.Id);
        builder.Property(category => category.Id).ValueGeneratedNever();

        builder.Property(category => category.Name).HasMaxLength(Category.NameMaxLength).IsRequired();
        builder.Property(category => category.Description).HasMaxLength(Category.DescriptionMaxLength).IsRequired();
        builder.Property(category => category.IsActive).IsRequired();

        builder.Property(category => category.DifficultyLevel)
            .HasConversion(level => level.Value, value => DifficultyLevel.Create(value).Value)
            .HasColumnName("DifficultyLevel")
            .IsRequired();

        builder.Property(category => category.PrizeAmount)
            .HasConversion(prize => prize.Amount, amount => Prize.Create(amount).Value)
            .HasColumnName("PrizeAmount")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.HasIndex(category => category.Name).IsUnique();

        builder.HasMany(category => category.Questions)
            .WithOne()
            .HasForeignKey(question => question.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Metadata.FindNavigation(nameof(Category.Questions))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Ignore(category => category.DomainEvents);

        builder.HasData(QuizGameSeedData.CategoryRows);
    }
}

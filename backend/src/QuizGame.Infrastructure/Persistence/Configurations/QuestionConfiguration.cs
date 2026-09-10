using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuizGame.Domain.Questions;
using QuizGame.Domain.ValueObjects;
using QuizGame.Infrastructure.Persistence.Seed;

namespace QuizGame.Infrastructure.Persistence.Configurations;

public sealed class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.ToTable("Questions");
        builder.HasKey(question => question.Id);
        builder.Property(question => question.Id).ValueGeneratedNever();

        builder.Property(question => question.CategoryId).IsRequired();
        builder.Property(question => question.IsActive).IsRequired();

        builder.Property(question => question.Text)
            .HasConversion(text => text.Value, value => QuestionText.Create(value).Value)
            .HasColumnName("Text")
            .HasMaxLength(QuestionText.MaxLength)
            .IsRequired();

        builder.HasMany(question => question.Answers)
            .WithOne()
            .HasForeignKey(answer => answer.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(Question.Answers))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasIndex(question => new { question.CategoryId, question.IsActive });

        builder.Ignore(question => question.DomainEvents);

        builder.HasData(QuizGameSeedData.QuestionRows);
    }
}

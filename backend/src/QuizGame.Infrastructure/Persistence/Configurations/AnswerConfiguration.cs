using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuizGame.Domain.Questions;
using QuizGame.Domain.ValueObjects;
using QuizGame.Infrastructure.Persistence.Seed;

namespace QuizGame.Infrastructure.Persistence.Configurations;

public sealed class AnswerConfiguration : IEntityTypeConfiguration<Answer>
{
    public void Configure(EntityTypeBuilder<Answer> builder)
    {
        builder.ToTable("Answers");
        builder.HasKey(answer => answer.Id);
        builder.Property(answer => answer.Id).ValueGeneratedNever();

        builder.Property(answer => answer.QuestionId).IsRequired();
        builder.Property(answer => answer.IsCorrect).IsRequired();

        builder.Property(answer => answer.Text)
            .HasConversion(text => text.Value, value => AnswerText.Create(value).Value)
            .HasColumnName("Text")
            .HasMaxLength(AnswerText.MaxLength)
            .IsRequired();

        builder.Ignore(answer => answer.DomainEvents);

        builder.HasData(QuizGameSeedData.AnswerRows);
    }
}

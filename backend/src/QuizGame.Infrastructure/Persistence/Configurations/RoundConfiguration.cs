using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuizGame.Domain.Games;
using QuizGame.Domain.ValueObjects;

namespace QuizGame.Infrastructure.Persistence.Configurations;

public sealed class RoundConfiguration : IEntityTypeConfiguration<Round>
{
    public void Configure(EntityTypeBuilder<Round> builder)
    {
        builder.ToTable("Rounds");
        builder.HasKey(round => round.Id);
        builder.Property(round => round.Id).ValueGeneratedNever();

        builder.Property(round => round.GameId).IsRequired();
        builder.Property(round => round.QuestionId).IsRequired();
        builder.Property(round => round.CorrectAnswerId).IsRequired();
        builder.Property(round => round.SelectedAnswerId);
        builder.Property(round => round.DeadlineUtc).IsRequired();
        builder.Property(round => round.AnsweredAtUtc);

        builder.Property(round => round.Number)
            .HasConversion(
                number => number.Value,
                value => RoundNumber.Create(value, GameSettings.Default.TotalRounds).Value)
            .HasColumnName("Number")
            .IsRequired();

        builder.Property(round => round.PrizeAtStake)
            .HasConversion(prize => prize.Amount, amount => Prize.Create(amount).Value)
            .HasColumnName("PrizeAtStake")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(round => round.Outcome)
            .HasConversion<string>()
            .HasColumnName("Outcome")
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(round => new { round.GameId, round.Number }).IsUnique();

        builder.Ignore(round => round.DomainEvents);
        builder.Ignore(round => round.IsAnswered);
    }
}

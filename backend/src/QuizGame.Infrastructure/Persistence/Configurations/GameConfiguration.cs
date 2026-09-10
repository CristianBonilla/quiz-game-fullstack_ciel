using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuizGame.Domain.Games;
using QuizGame.Domain.ValueObjects;

namespace QuizGame.Infrastructure.Persistence.Configurations;

public sealed class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.ToTable("Games");
        builder.HasKey(game => game.Id);
        builder.Property(game => game.Id).ValueGeneratedNever();

        builder.Property(game => game.PlayerName)
            .HasConversion(name => name.Value, value => PlayerName.Create(value).Value)
            .HasColumnName("PlayerName")
            .HasMaxLength(PlayerName.MaxLength)
            .IsRequired();

        builder.Property(game => game.Status)
            .HasConversion<string>()
            .HasColumnName("Status")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(game => game.CurrentRound)
            .HasConversion(
                round => round.Value,
                value => RoundNumber.Create(value, GameSettings.Default.TotalRounds).Value)
            .HasColumnName("CurrentRound")
            .IsRequired();

        builder.Property(game => game.AccumulatedPrize)
            .HasConversion(prize => prize.Amount, amount => Prize.Create(amount).Value)
            .HasColumnName("AccumulatedPrize")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(game => game.StartedAtUtc).IsRequired();
        builder.Property(game => game.EndedAtUtc);

        builder.OwnsOne(game => game.Settings, settings =>
        {
            settings.Property(gameSettings => gameSettings.TotalRounds).HasColumnName("TotalRounds").IsRequired();
            settings.Property(gameSettings => gameSettings.QuestionTimeLimit)
                .HasColumnName("QuestionTimeLimit")
                .IsRequired();
            settings.Ignore(gameSettings => gameSettings.PrizeByRound);
        });

        builder.Property<byte[]>("RowVersion").IsRowVersion();

        builder.HasMany(game => game.Rounds)
            .WithOne()
            .HasForeignKey(round => round.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation(nameof(Game.Rounds))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.Ignore(game => game.DomainEvents);
        builder.Ignore(game => game.AskedQuestionIds);
    }
}

using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QuizGame.Api.Hubs;
using QuizGame.Application.Abstractions;
using QuizGame.Application.Abstractions.Messaging;
using QuizGame.Application.Abstractions.Notifications;
using QuizGame.Application.Features.Games;
using QuizGame.Application.Features.Games.ForceEndGame;
using QuizGame.Domain.Games;
using QuizGame.Domain.SeedWork;
using QuizGame.Infrastructure.Persistence;

namespace QuizGame.Api.BackgroundServices;

/// <summary>
/// A single shared PeriodicTimer polls all in-progress rounds every second: at this scale a
/// per-game timer would add lifecycle-management complexity without a measurable benefit.
/// </summary>
public sealed partial class GameTimeoutService(
    IDbContextFactory<QuizGameDbContext> contextFactory,
    IServiceScopeFactory scopeFactory,
    IHubContext<GameHub, IGameClient> hubContext,
    IClock clock,
    ILogger<GameTimeoutService> logger) : BackgroundService
{
    private static readonly TimeSpan TickInterval = TimeSpan.FromSeconds(1);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new(TickInterval);

        while (await timer.WaitForNextTickAsync(stoppingToken).ConfigureAwait(false))
        {
            await TickAsync(stoppingToken).ConfigureAwait(false);
        }
    }

    private async Task TickAsync(CancellationToken cancellationToken)
    {
        await using QuizGameDbContext context = await contextFactory
            .CreateDbContextAsync(cancellationToken)
            .ConfigureAwait(false);

        List<OpenRound> openRounds = await context.Games
            .AsNoTracking()
            .Where(game => game.Status == GameStatus.InProgress)
            .Join(
                context.Set<Round>().AsNoTracking().Where(round => round.Outcome == RoundOutcome.Pending),
                game => game.Id,
                round => round.GameId,
                (game, round) => new OpenRound(game.Id, round.Number.Value, round.DeadlineUtc))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        if (openRounds.Count == 0)
        {
            return;
        }

        await using AsyncServiceScope scope = scopeFactory.CreateAsyncScope();
        ISender sender = scope.ServiceProvider.GetRequiredService<ISender>();
        DateTime now = clock.UtcNow;

        foreach (OpenRound openRound in openRounds)
        {
            int secondsRemaining = (int)Math.Max(0, (openRound.DeadlineUtc - now).TotalSeconds);

            if (secondsRemaining > 0)
            {
                await hubContext.Clients.Group(GameHub.GroupName(openRound.GameId))
                    .TimeRemainingAsync(new TimeRemainingNotification(openRound.GameId, openRound.RoundNumber, secondsRemaining))
                    .ConfigureAwait(false);

                continue;
            }

            await ForceEndExpiredGameAsync(sender, openRound.GameId, cancellationToken).ConfigureAwait(false);
        }
    }

    private async Task ForceEndExpiredGameAsync(ISender sender, Guid gameId, CancellationToken cancellationToken)
    {
        Result<GameSummaryResponse> result = await sender
            .SendAsync(new ForceEndGameCommand(gameId, "Round time expired"), cancellationToken)
            .ConfigureAwait(false);

        if (result.IsFailure)
        {
            GameForceEndFailed(logger, gameId, result.Error.Code);
        }
    }

    private sealed record OpenRound(Guid GameId, int RoundNumber, DateTime DeadlineUtc);

    [LoggerMessage(
        EventId = 4000,
        Level = LogLevel.Warning,
        Message = "Failed to force-end expired game {GameId}: {ErrorCode}")]
    private static partial void GameForceEndFailed(ILogger logger, Guid gameId, string errorCode);
}

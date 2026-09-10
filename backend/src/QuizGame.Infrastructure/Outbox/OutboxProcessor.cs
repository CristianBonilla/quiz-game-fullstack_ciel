using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.CircuitBreaker;
using Polly.Registry;
using QuizGame.Application.Abstractions;
using QuizGame.Application.Abstractions.Configuration;
using QuizGame.Application.Abstractions.Notifications;
using QuizGame.Domain.Games;
using QuizGame.Domain.Games.Events;
using QuizGame.Domain.Questions;
using QuizGame.Infrastructure.Resilience;

namespace QuizGame.Infrastructure.Outbox;

public sealed partial class OutboxProcessor(
    IDbContextFactory<Persistence.QuizGameDbContext> contextFactory,
    IServiceScopeFactory scopeFactory,
    ResiliencePipelineProvider<string> pipelineProvider,
    IOptions<ResilienceOptions> resilienceOptions,
    IClock clock,
    ILogger<OutboxProcessor> logger) : BackgroundService
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly OutboxResilienceOptions _options = resilienceOptions.Value.Outbox;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new(TimeSpan.FromSeconds(_options.PollingIntervalSeconds));

        while (await timer.WaitForNextTickAsync(stoppingToken).ConfigureAwait(false))
        {
            await ProcessPendingMessagesAsync(stoppingToken).ConfigureAwait(false);
        }
    }

    private async Task ProcessPendingMessagesAsync(CancellationToken cancellationToken)
    {
        await using Persistence.QuizGameDbContext context = await contextFactory
            .CreateDbContextAsync(cancellationToken)
            .ConfigureAwait(false);

        List<OutboxMessage> pending = await context.OutboxMessages
            .Where(message => message.ProcessedOnUtc == null && message.AttemptCount < _options.MaxRetryAttempts)
            .OrderBy(message => message.OccurredOnUtc)
            .Take(_options.BatchSize)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        if (pending.Count == 0)
        {
            return;
        }

        await using AsyncServiceScope scope = scopeFactory.CreateAsyncScope();
        IGameNotifier notifier = scope.ServiceProvider.GetRequiredService<IGameNotifier>();
        ResiliencePipeline pipeline = pipelineProvider.GetPipeline(ResiliencePipelineNames.OutboxDispatch);

        foreach (OutboxMessage message in pending)
        {
            try
            {
                await pipeline.ExecuteAsync(
                    async token => await DispatchAsync(message, context, notifier, token).ConfigureAwait(false),
                    cancellationToken).ConfigureAwait(false);

                message.MarkProcessed(clock.UtcNow);
            }
            catch (BrokenCircuitException)
            {
                OutboxCircuitOpen(logger);
                break;
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                message.MarkFailed(exception.Message);
                OutboxDispatchFailed(logger, message.Id, exception);
            }
        }

        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    private static async Task DispatchAsync(
        OutboxMessage message,
        Persistence.QuizGameDbContext context,
        IGameNotifier notifier,
        CancellationToken cancellationToken)
    {
        try
        {
            await DispatchCoreAsync(message, context, notifier, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception) when (exception is not OperationCanceledException and not TransientDispatchException)
        {
            throw new TransientDispatchException($"Dispatch of outbox message '{message.Id}' failed.", exception);
        }
    }

    private static async Task DispatchCoreAsync(
        OutboxMessage message,
        Persistence.QuizGameDbContext context,
        IGameNotifier notifier,
        CancellationToken cancellationToken)
    {
        Type? eventType = Type.GetType(message.Type);
        if (eventType is null)
        {
            return;
        }

        object? domainEvent = JsonSerializer.Deserialize(message.Content, eventType, SerializerOptions);
        if (domainEvent is null)
        {
            return;
        }

        switch (domainEvent)
        {
            case GameStartedDomainEvent gameStarted:
                await notifier.NotifyGameStartedAsync(
                    new GameStartedNotification(
                        gameStarted.GameId,
                        gameStarted.PlayerName,
                        gameStarted.TotalRounds,
                        gameStarted.OccurredOnUtc),
                    cancellationToken).ConfigureAwait(false);
                break;
            case RoundStartedDomainEvent roundStarted:
                await NotifyRoundStartedAsync(roundStarted, context, notifier, cancellationToken)
                    .ConfigureAwait(false);
                break;
            case AnswerSubmittedDomainEvent answerSubmitted:
                await NotifyAnswerSubmittedAsync(answerSubmitted, context, notifier, cancellationToken)
                    .ConfigureAwait(false);
                break;
            case RoundAdvancedDomainEvent roundAdvanced:
                await notifier.NotifyRoundAdvancedAsync(
                    new RoundAdvancedNotification(
                        roundAdvanced.GameId,
                        roundAdvanced.PreviousRoundNumber,
                        roundAdvanced.CurrentRoundNumber,
                        roundAdvanced.AccumulatedPrize),
                    cancellationToken).ConfigureAwait(false);
                break;
            case PrizeAccumulatedDomainEvent prizeAccumulated:
                await notifier.NotifyPrizeAccumulatedAsync(
                    new PrizeAccumulatedNotification(
                        prizeAccumulated.GameId,
                        prizeAccumulated.RoundNumber,
                        prizeAccumulated.PrizeWon,
                        prizeAccumulated.AccumulatedPrize),
                    cancellationToken).ConfigureAwait(false);
                break;
            case GameWonDomainEvent gameWon:
                await notifier.NotifyGameEndedAsync(
                    new GameEndedNotification(
                        gameWon.GameId,
                        gameWon.PlayerName,
                        nameof(GameStatus.Won),
                        gameWon.FinalPrize,
                        gameWon.OccurredOnUtc),
                    cancellationToken).ConfigureAwait(false);
                break;
            case GameLostDomainEvent gameLost:
                await notifier.NotifyGameEndedAsync(
                    new GameEndedNotification(
                        gameLost.GameId,
                        gameLost.PlayerName,
                        nameof(GameStatus.Lost),
                        0m,
                        gameLost.OccurredOnUtc),
                    cancellationToken).ConfigureAwait(false);
                break;
            case GameWithdrawnDomainEvent gameWithdrawn:
                await notifier.NotifyGameEndedAsync(
                    new GameEndedNotification(
                        gameWithdrawn.GameId,
                        gameWithdrawn.PlayerName,
                        nameof(GameStatus.Withdrawn),
                        gameWithdrawn.AccumulatedPrize,
                        gameWithdrawn.OccurredOnUtc),
                    cancellationToken).ConfigureAwait(false);
                break;
            case GameForciblyEndedDomainEvent forcedEnd:
                await notifier.NotifyGameEndedAsync(
                    new GameEndedNotification(
                        forcedEnd.GameId,
                        forcedEnd.PlayerName,
                        nameof(GameStatus.ForcedEnd),
                        0m,
                        forcedEnd.OccurredOnUtc),
                    cancellationToken).ConfigureAwait(false);
                break;
        }
    }

    private static async Task NotifyRoundStartedAsync(
        RoundStartedDomainEvent domainEvent,
        Persistence.QuizGameDbContext context,
        IGameNotifier notifier,
        CancellationToken cancellationToken)
    {
        Question? question = await context.Questions
            .AsNoTracking()
            .Include(found => found.Answers)
            .FirstOrDefaultAsync(found => found.Id == domainEvent.QuestionId, cancellationToken)
            .ConfigureAwait(false);

        if (question is null)
        {
            return;
        }

        await notifier.NotifyRoundStartedAsync(
            new RoundStartedNotification(
                domainEvent.GameId,
                domainEvent.RoundNumber,
                question.Id,
                question.Text.Value,
                [.. question.Answers.Select(answer => new NotificationAnswer(answer.Id, answer.Text.Value))],
                domainEvent.PrizeAtStake,
                domainEvent.DeadlineUtc),
            cancellationToken).ConfigureAwait(false);
    }

    private static async Task NotifyAnswerSubmittedAsync(
        AnswerSubmittedDomainEvent domainEvent,
        Persistence.QuizGameDbContext context,
        IGameNotifier notifier,
        CancellationToken cancellationToken)
    {
        Game? game = await context.Games
            .AsNoTracking()
            .FirstOrDefaultAsync(found => found.Id == domainEvent.GameId, cancellationToken)
            .ConfigureAwait(false);

        if (game is null)
        {
            return;
        }

        Question? question = await context.Questions
            .AsNoTracking()
            .Include(found => found.Answers)
            .FirstOrDefaultAsync(found => found.Id == domainEvent.QuestionId, cancellationToken)
            .ConfigureAwait(false);

        await notifier.NotifyAnswerEvaluatedAsync(
            new AnswerEvaluatedNotification(
                domainEvent.GameId,
                domainEvent.RoundNumber,
                domainEvent.IsCorrect,
                question?.CorrectAnswerId ?? Guid.Empty,
                game.AccumulatedPrize.Amount,
                game.Status.ToString()),
            cancellationToken).ConfigureAwait(false);
    }

    [LoggerMessage(EventId = 2000, Level = LogLevel.Error, Message = "Failed to dispatch outbox message {MessageId}")]
    private static partial void OutboxDispatchFailed(ILogger logger, Guid messageId, Exception exception);

    [LoggerMessage(
        EventId = 2001,
        Level = LogLevel.Warning,
        Message = "Outbox circuit is open; deferring this batch until it closes")]
    private static partial void OutboxCircuitOpen(ILogger logger);
}

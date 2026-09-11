using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using QuizGame.Api.Hubs;
using QuizGame.Application.Abstractions.Notifications;

namespace QuizGame.Api.Notifications;

public sealed partial class SignalRGameNotifier(
    IHubContext<GameHub, IGameClient> hubContext,
    ILogger<SignalRGameNotifier> logger) : IGameNotifier
{
    public Task NotifyGameStartedAsync(GameStartedNotification notification, CancellationToken cancellationToken)
    {
        LogGameStarted(logger, notification.GameId, notification.PlayerName, notification.TotalRounds);
        return Clients(notification.GameId).GameStartedAsync(notification);
    }

    public Task NotifyRoundStartedAsync(RoundStartedNotification notification, CancellationToken cancellationToken)
    {
        LogRoundStarted(logger, notification.GameId, notification.RoundNumber, notification.QuestionId, notification.PrizeAtStake);
        return Clients(notification.GameId).RoundStartedAsync(notification);
    }

    public Task NotifyAnswerEvaluatedAsync(
        AnswerEvaluatedNotification notification,
        CancellationToken cancellationToken)
    {
        LogAnswerEvaluated(logger, notification.GameId, notification.RoundNumber, notification.IsCorrect, notification.Status, notification.AccumulatedPrize);
        return Clients(notification.GameId).AnswerEvaluatedAsync(notification);
    }

    public Task NotifyRoundAdvancedAsync(RoundAdvancedNotification notification, CancellationToken cancellationToken)
    {
        LogRoundAdvanced(logger, notification.GameId, notification.PreviousRoundNumber, notification.CurrentRoundNumber, notification.AccumulatedPrize);
        return Clients(notification.GameId).RoundAdvancedAsync(notification);
    }

    public Task NotifyPrizeAccumulatedAsync(
        PrizeAccumulatedNotification notification,
        CancellationToken cancellationToken)
    {
        LogPrizeAccumulated(logger, notification.GameId, notification.RoundNumber, notification.PrizeWon, notification.AccumulatedPrize);
        return Clients(notification.GameId).PrizeAccumulatedAsync(notification);
    }

    public Task NotifyGameEndedAsync(GameEndedNotification notification, CancellationToken cancellationToken)
    {
        LogGameEnded(logger, notification.GameId, notification.PlayerName, notification.Status, notification.FinalPrize);
        return Clients(notification.GameId).GameEndedAsync(notification);
    }

    public Task NotifyTimeRemainingAsync(
        TimeRemainingNotification notification,
        CancellationToken cancellationToken)
    {
        LogTimeRemaining(logger, notification.GameId, notification.RoundNumber, notification.SecondsRemaining);
        return Clients(notification.GameId).TimeRemainingAsync(notification);
    }

    private IGameClient Clients(Guid gameId) => hubContext.Clients.Group(GameHub.GroupName(gameId));

    [LoggerMessage(EventId = 3100, Level = LogLevel.Information, Message = "[SignalR Hub] Broadcasted GameStarted: Game {GameId}, Player '{PlayerName}', TotalRounds: {TotalRounds}, GameStatus is now InProgress")]
    private static partial void LogGameStarted(ILogger logger, Guid gameId, string playerName, int totalRounds);

    [LoggerMessage(EventId = 3101, Level = LogLevel.Information, Message = "[SignalR Hub] Broadcasted RoundStarted: Game {GameId}, Round: {RoundNumber}, Question: {QuestionId}, PrizeAtStake: {PrizeAtStake}")]
    private static partial void LogRoundStarted(ILogger logger, Guid gameId, int roundNumber, Guid questionId, decimal prizeAtStake);

    [LoggerMessage(EventId = 3102, Level = LogLevel.Information, Message = "[SignalR Hub] Broadcasted AnswerEvaluated: Game {GameId}, Round: {RoundNumber}, IsCorrect: {IsCorrect}, GameStatus is now {Status}, AccumulatedPrize: {AccumulatedPrize}")]
    private static partial void LogAnswerEvaluated(ILogger logger, Guid gameId, int roundNumber, bool isCorrect, string status, decimal accumulatedPrize);

    [LoggerMessage(EventId = 3103, Level = LogLevel.Information, Message = "[SignalR Hub] Broadcasted RoundAdvanced: Game {GameId}, Advanced from Round {PreviousRound} to {CurrentRound}, AccumulatedPrize: {AccumulatedPrize}")]
    private static partial void LogRoundAdvanced(ILogger logger, Guid gameId, int previousRound, int currentRound, decimal accumulatedPrize);

    [LoggerMessage(EventId = 3104, Level = LogLevel.Information, Message = "[SignalR Hub] Broadcasted PrizeAccumulated: Game {GameId}, Round: {RoundNumber}, PrizeWon: {PrizeWon}, AccumulatedPrize: {AccumulatedPrize}")]
    private static partial void LogPrizeAccumulated(ILogger logger, Guid gameId, int roundNumber, decimal prizeWon, decimal accumulatedPrize);

    [LoggerMessage(EventId = 3105, Level = LogLevel.Information, Message = "[SignalR Hub] Broadcasted GameEnded: Game {GameId}, Player '{PlayerName}', FinalStatus: {Status}, FinalPrize: {FinalPrize}")]
    private static partial void LogGameEnded(ILogger logger, Guid gameId, string playerName, string status, decimal finalPrize);

    [LoggerMessage(EventId = 3106, Level = LogLevel.Debug, Message = "[SignalR Hub] Broadcasted TimeRemaining: Game {GameId}, Round: {RoundNumber}, SecondsRemaining: {SecondsRemaining}s")]
    private static partial void LogTimeRemaining(ILogger logger, Guid gameId, int roundNumber, int secondsRemaining);
}

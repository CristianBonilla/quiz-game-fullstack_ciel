namespace QuizGame.Application.Abstractions.Notifications;

public sealed class NoOpGameNotifier : IGameNotifier
{
    public Task NotifyGameStartedAsync(
        GameStartedNotification notification,
        CancellationToken cancellationToken) => Task.CompletedTask;

    public Task NotifyRoundStartedAsync(
        RoundStartedNotification notification,
        CancellationToken cancellationToken) => Task.CompletedTask;

    public Task NotifyAnswerEvaluatedAsync(
        AnswerEvaluatedNotification notification,
        CancellationToken cancellationToken) => Task.CompletedTask;

    public Task NotifyRoundAdvancedAsync(
        RoundAdvancedNotification notification,
        CancellationToken cancellationToken) => Task.CompletedTask;

    public Task NotifyPrizeAccumulatedAsync(
        PrizeAccumulatedNotification notification,
        CancellationToken cancellationToken) => Task.CompletedTask;

    public Task NotifyGameEndedAsync(
        GameEndedNotification notification,
        CancellationToken cancellationToken) => Task.CompletedTask;

    public Task NotifyTimeRemainingAsync(
        TimeRemainingNotification notification,
        CancellationToken cancellationToken) => Task.CompletedTask;
}

namespace QuizGame.Application.Abstractions.Notifications;

public interface IGameNotifier
{
    Task NotifyGameStartedAsync(GameStartedNotification notification, CancellationToken cancellationToken);

    Task NotifyRoundStartedAsync(RoundStartedNotification notification, CancellationToken cancellationToken);

    Task NotifyAnswerEvaluatedAsync(AnswerEvaluatedNotification notification, CancellationToken cancellationToken);

    Task NotifyRoundAdvancedAsync(RoundAdvancedNotification notification, CancellationToken cancellationToken);

    Task NotifyPrizeAccumulatedAsync(PrizeAccumulatedNotification notification, CancellationToken cancellationToken);

    Task NotifyGameEndedAsync(GameEndedNotification notification, CancellationToken cancellationToken);

    Task NotifyTimeRemainingAsync(TimeRemainingNotification notification, CancellationToken cancellationToken);
}

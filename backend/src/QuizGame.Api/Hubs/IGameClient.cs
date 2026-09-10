using QuizGame.Application.Abstractions.Notifications;

namespace QuizGame.Api.Hubs;

public interface IGameClient
{
    Task GameStartedAsync(GameStartedNotification notification);

    Task RoundStartedAsync(RoundStartedNotification notification);

    Task AnswerEvaluatedAsync(AnswerEvaluatedNotification notification);

    Task RoundAdvancedAsync(RoundAdvancedNotification notification);

    Task PrizeAccumulatedAsync(PrizeAccumulatedNotification notification);

    Task GameEndedAsync(GameEndedNotification notification);

    Task TimeRemainingAsync(TimeRemainingNotification notification);
}

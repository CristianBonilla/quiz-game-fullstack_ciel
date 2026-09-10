using Microsoft.AspNetCore.SignalR;
using QuizGame.Api.Hubs;
using QuizGame.Application.Abstractions.Notifications;

namespace QuizGame.Api.Notifications;

public sealed class SignalRGameNotifier(IHubContext<GameHub, IGameClient> hubContext) : IGameNotifier
{
    public Task NotifyGameStartedAsync(GameStartedNotification notification, CancellationToken cancellationToken) =>
        Clients(notification.GameId).GameStartedAsync(notification);

    public Task NotifyRoundStartedAsync(RoundStartedNotification notification, CancellationToken cancellationToken) =>
        Clients(notification.GameId).RoundStartedAsync(notification);

    public Task NotifyAnswerEvaluatedAsync(
        AnswerEvaluatedNotification notification,
        CancellationToken cancellationToken) =>
        Clients(notification.GameId).AnswerEvaluatedAsync(notification);

    public Task NotifyRoundAdvancedAsync(RoundAdvancedNotification notification, CancellationToken cancellationToken) =>
        Clients(notification.GameId).RoundAdvancedAsync(notification);

    public Task NotifyPrizeAccumulatedAsync(
        PrizeAccumulatedNotification notification,
        CancellationToken cancellationToken) =>
        Clients(notification.GameId).PrizeAccumulatedAsync(notification);

    public Task NotifyGameEndedAsync(GameEndedNotification notification, CancellationToken cancellationToken) =>
        Clients(notification.GameId).GameEndedAsync(notification);

    public Task NotifyTimeRemainingAsync(
        TimeRemainingNotification notification,
        CancellationToken cancellationToken) =>
        Clients(notification.GameId).TimeRemainingAsync(notification);

    private IGameClient Clients(Guid gameId) => hubContext.Clients.Group(GameHub.GroupName(gameId));
}

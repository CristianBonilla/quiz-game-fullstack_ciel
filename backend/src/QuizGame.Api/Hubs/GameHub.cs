using Microsoft.AspNetCore.SignalR;
using QuizGame.Application.Abstractions.Messaging;
using QuizGame.Application.Features.Games;
using QuizGame.Application.Features.Games.AnswerQuestion;
using QuizGame.Application.Features.Games.GetGameState;
using QuizGame.Application.Features.Games.WithdrawGame;
using QuizGame.Domain.SeedWork;

namespace QuizGame.Api.Hubs;

public sealed partial class GameHub(ISender sender, ILogger<GameHub> logger) : Hub<IGameClient>
{
    public async Task<HubResponse<GameStateResponse>> JoinGameAsync(Guid gameId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(gameId), Context.ConnectionAborted)
            .ConfigureAwait(false);

        Result<GameStateResponse> result = await sender
            .QueryAsync(new GetGameStateQuery(gameId), Context.ConnectionAborted)
            .ConfigureAwait(false);

        return result.ToHubResponse();
    }

    public async Task<HubResponse<AnswerQuestionResponse>> SubmitAnswerAsync(SubmitAnswerRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        Result<AnswerQuestionResponse> result = await sender
            .SendAsync(
                new AnswerQuestionCommand(request.GameId, request.AnswerId, request.RequestId),
                Context.ConnectionAborted)
            .ConfigureAwait(false);

        return result.ToHubResponse();
    }

    public async Task<HubResponse<GameSummaryResponse>> WithdrawAsync(WithdrawRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        Result<GameSummaryResponse> result = await sender
            .SendAsync(new WithdrawGameCommand(request.GameId), Context.ConnectionAborted)
            .ConfigureAwait(false);

        return result.ToHubResponse();
    }

    public Task LeaveGameAsync(Guid gameId) =>
        Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName(gameId), Context.ConnectionAborted);

    public override Task OnConnectedAsync()
    {
        ClientConnected(logger, Context.ConnectionId);
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        ClientDisconnected(logger, Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }

    public static string GroupName(Guid gameId) => $"game-{gameId}";

    [LoggerMessage(EventId = 3000, Level = LogLevel.Information, Message = "SignalR client {ConnectionId} connected")]
    private static partial void ClientConnected(ILogger logger, string connectionId);

    [LoggerMessage(EventId = 3001, Level = LogLevel.Information, Message = "SignalR client {ConnectionId} disconnected")]
    private static partial void ClientDisconnected(ILogger logger, string connectionId);
}

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
        ClientJoinedGame(logger, Context.ConnectionId, gameId);

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

        ClientSubmittedAnswer(logger, Context.ConnectionId, request.GameId, request.AnswerId);

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

        ClientWithdrew(logger, Context.ConnectionId, request.GameId);

        Result<GameSummaryResponse> result = await sender
            .SendAsync(new WithdrawGameCommand(request.GameId), Context.ConnectionAborted)
            .ConfigureAwait(false);

        return result.ToHubResponse();
    }

    public Task LeaveGameAsync(Guid gameId)
    {
        ClientLeftGame(logger, Context.ConnectionId, gameId);
        return Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName(gameId), Context.ConnectionAborted);
    }

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

    [LoggerMessage(EventId = 3000, Level = LogLevel.Information, Message = "[SignalR Hub] Client {ConnectionId} connected")]
    private static partial void ClientConnected(ILogger logger, string connectionId);

    [LoggerMessage(EventId = 3001, Level = LogLevel.Information, Message = "[SignalR Hub] Client {ConnectionId} disconnected")]
    private static partial void ClientDisconnected(ILogger logger, string connectionId);

    [LoggerMessage(EventId = 3002, Level = LogLevel.Information, Message = "[SignalR Hub] Client {ConnectionId} joined game group {GameId}")]
    private static partial void ClientJoinedGame(ILogger logger, string connectionId, Guid gameId);

    [LoggerMessage(EventId = 3003, Level = LogLevel.Information, Message = "[SignalR Hub] Client {ConnectionId} submitted answer {AnswerId} for game {GameId}")]
    private static partial void ClientSubmittedAnswer(ILogger logger, string connectionId, Guid gameId, Guid answerId);

    [LoggerMessage(EventId = 3004, Level = LogLevel.Information, Message = "[SignalR Hub] Client {ConnectionId} requested withdrawal from game {GameId}")]
    private static partial void ClientWithdrew(ILogger logger, string connectionId, Guid gameId);

    [LoggerMessage(EventId = 3005, Level = LogLevel.Information, Message = "[SignalR Hub] Client {ConnectionId} left game group {GameId}")]
    private static partial void ClientLeftGame(ILogger logger, string connectionId, Guid gameId);
}

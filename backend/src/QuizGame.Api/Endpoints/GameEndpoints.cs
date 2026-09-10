using QuizGame.Api.Extensions;
using QuizGame.Application.Abstractions.Messaging;
using QuizGame.Application.Features.Games;
using QuizGame.Application.Features.Games.ConfigureGame;
using QuizGame.Application.Features.Games.GetGameState;
using QuizGame.Application.Features.Games.GetGameSummary;
using QuizGame.Application.Features.Games.StartGame;
using QuizGame.Domain.SeedWork;

namespace QuizGame.Api.Endpoints;

public sealed record StartGameRequest(string PlayerName);

public sealed class GameEndpoints : IEndpointModule
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        RouteGroupBuilder group = app.MapGroup("/api/v1/games")
            .WithTags("Games")
            .RequireRateLimiting(RateLimiterPolicies.Api);

        group.MapPost("/", StartGameAsync)
            .WithName("StartGame")
            .Produces<GameStateResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem();

        group.MapGet("/settings", GetSettingsAsync)
            .WithName("GetGameSettings")
            .Produces<GameConfigurationResponse>();

        group.MapGet("/{id:guid}", GetGameStateAsync)
            .WithName("GetGameState")
            .Produces<GameStateResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/{id:guid}/summary", GetGameSummaryAsync)
            .WithName("GetGameSummary")
            .Produces<GameSummaryResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> StartGameAsync(
        StartGameRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        Result<GameStateResponse> result = await sender
            .SendAsync(new StartGameCommand(request.PlayerName), cancellationToken)
            .ConfigureAwait(false);

        return result.ToCreatedResult(result.IsSuccess ? $"/api/v1/games/{result.Value.GameId}" : string.Empty);
    }

    private static async Task<IResult> GetSettingsAsync(ISender sender, CancellationToken cancellationToken)
    {
        Result<GameConfigurationResponse> result = await sender
            .QueryAsync(new ConfigureGameQuery(), cancellationToken)
            .ConfigureAwait(false);

        return result.ToOkResult();
    }

    private static async Task<IResult> GetGameStateAsync(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        Result<GameStateResponse> result = await sender
            .QueryAsync(new GetGameStateQuery(id), cancellationToken)
            .ConfigureAwait(false);

        return result.ToOkResult();
    }

    private static async Task<IResult> GetGameSummaryAsync(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        Result<GameSummaryResponse> result = await sender
            .QueryAsync(new GetGameSummaryQuery(id), cancellationToken)
            .ConfigureAwait(false);

        return result.ToOkResult();
    }
}

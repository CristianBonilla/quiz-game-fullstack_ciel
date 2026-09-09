using QuizGame.Application.Abstractions;
using QuizGame.Application.Abstractions.Messaging;
using QuizGame.Application.Abstractions.Persistence;
using QuizGame.Domain.Games;
using QuizGame.Domain.Questions;
using QuizGame.Domain.SeedWork;
using QuizGame.Domain.ValueObjects;

namespace QuizGame.Application.Features.Games.StartGame;

public sealed class StartGameCommandHandler(
    IGameRepository games,
    GameSettingsFactory settingsFactory,
    RoundAssignmentService roundAssignment,
    IClock clock) : ICommandHandler<StartGameCommand, GameStateResponse>
{
    public async Task<Result<GameStateResponse>> HandleAsync(
        StartGameCommand command,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        Result<PlayerName> playerName = PlayerName.Create(command.PlayerName);
        if (playerName.IsFailure)
        {
            return Result.Failure<GameStateResponse>(playerName.Error);
        }

        Result<GameSettings> settings = settingsFactory.Create();
        if (settings.IsFailure)
        {
            return Result.Failure<GameStateResponse>(settings.Error);
        }

        Result<Game> game = Game.Create(Guid.NewGuid(), playerName.Value, settings.Value, clock.UtcNow);
        if (game.IsFailure)
        {
            return Result.Failure<GameStateResponse>(game.Error);
        }

        Result<Question> question = await roundAssignment
            .AssignNextRoundAsync(game.Value, cancellationToken)
            .ConfigureAwait(false);

        if (question.IsFailure)
        {
            return Result.Failure<GameStateResponse>(question.Error);
        }

        games.Add(game.Value);

        return game.Value.ToStateResponse(question.Value);
    }
}

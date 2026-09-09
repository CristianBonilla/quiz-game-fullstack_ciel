using QuizGame.Application.Abstractions;
using QuizGame.Application.Abstractions.Messaging;
using QuizGame.Application.Abstractions.Persistence;
using QuizGame.Domain.Games;
using QuizGame.Domain.Games.Errors;
using QuizGame.Domain.SeedWork;

namespace QuizGame.Application.Features.Games.ForceEndGame;

public sealed class ForceEndGameCommandHandler(IGameRepository games, IClock clock)
    : ICommandHandler<ForceEndGameCommand, GameSummaryResponse>
{
    public async Task<Result<GameSummaryResponse>> HandleAsync(
        ForceEndGameCommand command,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        Game? game = await games.GetByIdAsync(command.GameId, cancellationToken).ConfigureAwait(false);
        if (game is null)
        {
            return Result.Failure<GameSummaryResponse>(GameErrors.NotFound(command.GameId));
        }

        Result forcedEnd = game.ForceEnd(command.Reason, clock.UtcNow);

        return forcedEnd.IsFailure
            ? Result.Failure<GameSummaryResponse>(forcedEnd.Error)
            : game.ToSummaryResponse();
    }
}

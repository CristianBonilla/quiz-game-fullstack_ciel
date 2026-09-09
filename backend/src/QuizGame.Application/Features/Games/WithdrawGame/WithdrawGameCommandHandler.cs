using QuizGame.Application.Abstractions;
using QuizGame.Application.Abstractions.Messaging;
using QuizGame.Application.Abstractions.Persistence;
using QuizGame.Domain.Games;
using QuizGame.Domain.Games.Errors;
using QuizGame.Domain.SeedWork;

namespace QuizGame.Application.Features.Games.WithdrawGame;

public sealed class WithdrawGameCommandHandler(IGameRepository games, IClock clock)
    : ICommandHandler<WithdrawGameCommand, GameSummaryResponse>
{
    public async Task<Result<GameSummaryResponse>> HandleAsync(
        WithdrawGameCommand command,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        Game? game = await games.GetByIdAsync(command.GameId, cancellationToken).ConfigureAwait(false);
        if (game is null)
        {
            return Result.Failure<GameSummaryResponse>(GameErrors.NotFound(command.GameId));
        }

        Result withdrawal = game.Withdraw(clock.UtcNow);

        return withdrawal.IsFailure
            ? Result.Failure<GameSummaryResponse>(withdrawal.Error)
            : game.ToSummaryResponse();
    }
}

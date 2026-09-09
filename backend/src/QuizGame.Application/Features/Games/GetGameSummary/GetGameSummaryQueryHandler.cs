using QuizGame.Application.Abstractions.Messaging;
using QuizGame.Application.Abstractions.Persistence;
using QuizGame.Domain.Games;
using QuizGame.Domain.Games.Errors;
using QuizGame.Domain.SeedWork;

namespace QuizGame.Application.Features.Games.GetGameSummary;

public sealed class GetGameSummaryQueryHandler(IGameRepository games)
    : IQueryHandler<GetGameSummaryQuery, GameSummaryResponse>
{
    public async Task<Result<GameSummaryResponse>> HandleAsync(
        GetGameSummaryQuery query,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        Game? game = await games.GetByIdAsync(query.GameId, cancellationToken).ConfigureAwait(false);

        return game is null
            ? Result.Failure<GameSummaryResponse>(GameErrors.NotFound(query.GameId))
            : game.ToSummaryResponse();
    }
}

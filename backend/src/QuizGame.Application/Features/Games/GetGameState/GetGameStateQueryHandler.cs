using QuizGame.Application.Abstractions.Messaging;
using QuizGame.Application.Abstractions.Persistence;
using QuizGame.Domain.Games;
using QuizGame.Domain.Games.Errors;
using QuizGame.Domain.Questions;
using QuizGame.Domain.SeedWork;

namespace QuizGame.Application.Features.Games.GetGameState;

public sealed class GetGameStateQueryHandler(IGameRepository games, IQuestionRepository questions)
    : IQueryHandler<GetGameStateQuery, GameStateResponse>
{
    public async Task<Result<GameStateResponse>> HandleAsync(
        GetGameStateQuery query,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        Game? game = await games.GetByIdAsync(query.GameId, cancellationToken).ConfigureAwait(false);
        if (game is null)
        {
            return Result.Failure<GameStateResponse>(GameErrors.NotFound(query.GameId));
        }

        Round? openRound = game.Rounds.FirstOrDefault(round => !round.IsAnswered);
        Question? currentQuestion = openRound is null
            ? null
            : await questions.GetByIdAsync(openRound.QuestionId, cancellationToken).ConfigureAwait(false);

        return game.ToStateResponse(currentQuestion);
    }
}

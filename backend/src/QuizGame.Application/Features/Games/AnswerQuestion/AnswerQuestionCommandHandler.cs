using QuizGame.Application.Abstractions;
using QuizGame.Application.Abstractions.Messaging;
using QuizGame.Application.Abstractions.Persistence;
using QuizGame.Application.Features.Questions;
using QuizGame.Domain.Games;
using QuizGame.Domain.Games.Errors;
using QuizGame.Domain.Questions;
using QuizGame.Domain.SeedWork;

namespace QuizGame.Application.Features.Games.AnswerQuestion;

public sealed class AnswerQuestionCommandHandler(
    IGameRepository games,
    IQuestionRepository questions,
    RoundAssignmentService roundAssignment,
    IClock clock) : ICommandHandler<AnswerQuestionCommand, AnswerQuestionResponse>
{
    public async Task<Result<AnswerQuestionResponse>> HandleAsync(
        AnswerQuestionCommand command,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        Game? game = await games.GetByIdAsync(command.GameId, cancellationToken).ConfigureAwait(false);
        if (game is null)
        {
            return Result.Failure<AnswerQuestionResponse>(GameErrors.NotFound(command.GameId));
        }

        Round? openRound = game.Rounds.FirstOrDefault(round => !round.IsAnswered);
        if (openRound is null)
        {
            return Result.Failure<AnswerQuestionResponse>(GameErrors.NoOpenRound);
        }

        Question? asked = await questions
            .GetByIdAsync(openRound.QuestionId, cancellationToken)
            .ConfigureAwait(false);

        if (asked is null)
        {
            return Result.Failure<AnswerQuestionResponse>(QuestionErrors.NotFound(openRound.QuestionId));
        }

        if (!asked.HasAnswer(command.AnswerId))
        {
            return Result.Failure<AnswerQuestionResponse>(GameErrors.AnswerNotInQuestion);
        }

        int answeredRound = openRound.Number.Value;

        Result evaluation = game.Answer(command.AnswerId, clock.UtcNow);
        if (evaluation.IsFailure)
        {
            return Result.Failure<AnswerQuestionResponse>(evaluation.Error);
        }

        Question? nextQuestion = null;
        if (game.Status == GameStatus.InProgress)
        {
            Result<Question> assignment = await roundAssignment
                .AssignNextRoundAsync(game, cancellationToken)
                .ConfigureAwait(false);

            if (assignment.IsFailure)
            {
                return Result.Failure<AnswerQuestionResponse>(assignment.Error);
            }

            nextQuestion = assignment.Value;
        }

        Round? pendingRound = game.Rounds.FirstOrDefault(round => !round.IsAnswered);

        return new AnswerQuestionResponse(
            game.Id,
            answeredRound,
            openRound.Outcome == RoundOutcome.Correct,
            asked.CorrectAnswerId,
            game.Status.ToString(),
            game.AccumulatedPrize.Amount,
            game.CurrentRound.Value,
            pendingRound?.PrizeAtStake.Amount ?? 0m,
            nextQuestion?.ToPlayableResponse(),
            pendingRound?.DeadlineUtc);
    }
}

using QuizGame.Application.Abstractions.Messaging;
using QuizGame.Application.Abstractions.Persistence;
using QuizGame.Domain.Questions;
using QuizGame.Domain.SeedWork;

namespace QuizGame.Application.Features.Questions.DeactivateQuestion;

public sealed class DeactivateQuestionCommandHandler(IQuestionRepository questions)
    : ICommandHandler<DeactivateQuestionCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(
        DeactivateQuestionCommand command,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        Question? question = await questions
            .GetByIdAsync(command.QuestionId, cancellationToken)
            .ConfigureAwait(false);

        if (question is null)
        {
            return Result.Failure<Unit>(QuestionErrors.NotFound(command.QuestionId));
        }

        Result deactivation = question.Deactivate();

        return deactivation.IsFailure
            ? Result.Failure<Unit>(deactivation.Error)
            : Result.Success(Unit.Value);
    }
}

using QuizGame.Application.Abstractions.Messaging;
using QuizGame.Application.Abstractions.Persistence;
using QuizGame.Domain.Questions;
using QuizGame.Domain.SeedWork;
using QuizGame.Domain.ValueObjects;

namespace QuizGame.Application.Features.Questions.UpdateQuestion;

public sealed class UpdateQuestionCommandHandler(IQuestionRepository questions)
    : ICommandHandler<UpdateQuestionCommand, QuestionResponse>
{
    public async Task<Result<QuestionResponse>> HandleAsync(
        UpdateQuestionCommand command,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        Question? question = await questions
            .GetByIdAsync(command.QuestionId, cancellationToken)
            .ConfigureAwait(false);

        if (question is null)
        {
            return Result.Failure<QuestionResponse>(QuestionErrors.NotFound(command.QuestionId));
        }

        Result<QuestionText> text = QuestionText.Create(command.Text);
        if (text.IsFailure)
        {
            return Result.Failure<QuestionResponse>(text.Error);
        }

        Result<IReadOnlyCollection<AnswerCandidate>> candidates =
            AnswerDraftMapper.ToCandidates(command.Answers);

        if (candidates.IsFailure)
        {
            return Result.Failure<QuestionResponse>(candidates.Error);
        }

        Result update = question.Update(text.Value, candidates.Value);

        return update.IsFailure
            ? Result.Failure<QuestionResponse>(update.Error)
            : question.ToResponse();
    }
}

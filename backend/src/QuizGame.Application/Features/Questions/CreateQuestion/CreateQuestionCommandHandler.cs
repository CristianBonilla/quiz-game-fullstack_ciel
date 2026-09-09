using QuizGame.Application.Abstractions.Messaging;
using QuizGame.Application.Abstractions.Persistence;
using QuizGame.Domain.Categories;
using QuizGame.Domain.Questions;
using QuizGame.Domain.SeedWork;
using QuizGame.Domain.ValueObjects;

namespace QuizGame.Application.Features.Questions.CreateQuestion;

public sealed class CreateQuestionCommandHandler(
    IQuestionRepository questions,
    ICategoryRepository categories) : ICommandHandler<CreateQuestionCommand, QuestionResponse>
{
    public async Task<Result<QuestionResponse>> HandleAsync(
        CreateQuestionCommand command,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        Category? category = await categories
            .GetByIdAsync(command.CategoryId, cancellationToken)
            .ConfigureAwait(false);

        if (category is null)
        {
            return Result.Failure<QuestionResponse>(CategoryErrors.NotFound(command.CategoryId));
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

        Result<Question> question = Question.Create(
            Guid.NewGuid(),
            command.CategoryId,
            text.Value,
            candidates.Value);

        if (question.IsFailure)
        {
            return Result.Failure<QuestionResponse>(question.Error);
        }

        questions.Add(question.Value);

        return question.Value.ToResponse();
    }
}

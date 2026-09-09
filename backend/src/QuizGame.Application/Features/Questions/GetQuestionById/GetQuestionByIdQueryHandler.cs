using QuizGame.Application.Abstractions.Messaging;
using QuizGame.Application.Abstractions.Persistence;
using QuizGame.Domain.Questions;
using QuizGame.Domain.SeedWork;

namespace QuizGame.Application.Features.Questions.GetQuestionById;

public sealed class GetQuestionByIdQueryHandler(IQuestionRepository questions)
    : IQueryHandler<GetQuestionByIdQuery, QuestionResponse>
{
    public async Task<Result<QuestionResponse>> HandleAsync(
        GetQuestionByIdQuery query,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        Question? question = await questions
            .GetByIdAsync(query.QuestionId, cancellationToken)
            .ConfigureAwait(false);

        return question is null
            ? Result.Failure<QuestionResponse>(QuestionErrors.NotFound(query.QuestionId))
            : question.ToResponse();
    }
}

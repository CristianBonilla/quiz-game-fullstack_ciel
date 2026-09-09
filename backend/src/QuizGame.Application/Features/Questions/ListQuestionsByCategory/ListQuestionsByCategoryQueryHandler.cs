using QuizGame.Application.Abstractions.Messaging;
using QuizGame.Application.Abstractions.Persistence;
using QuizGame.Domain.Questions;
using QuizGame.Domain.SeedWork;

namespace QuizGame.Application.Features.Questions.ListQuestionsByCategory;

public sealed class ListQuestionsByCategoryQueryHandler(IQuestionRepository questions)
    : IQueryHandler<ListQuestionsByCategoryQuery, IReadOnlyCollection<QuestionResponse>>
{
    public async Task<Result<IReadOnlyCollection<QuestionResponse>>> HandleAsync(
        ListQuestionsByCategoryQuery query,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        IReadOnlyCollection<Question> found = await questions
            .ListByCategoryAsync(query.CategoryId, query.OnlyActive, cancellationToken)
            .ConfigureAwait(false);

        IReadOnlyCollection<QuestionResponse> response = [.. found.Select(question => question.ToResponse())];

        return Result.Success(response);
    }
}

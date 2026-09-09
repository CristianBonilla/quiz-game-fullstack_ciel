using QuizGame.Application.Abstractions;
using QuizGame.Application.Abstractions.Persistence;
using QuizGame.Domain.Questions;
using QuizGame.Domain.SeedWork;

namespace QuizGame.Application.Strategies;

public sealed class RandomQuestionSelectionStrategy(
    IQuestionRepository questions,
    IRandomProvider random) : IQuestionSelectionStrategy
{
    public async Task<Result<Question>> SelectAsync(
        QuestionSelectionContext context,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(context);

        IReadOnlyCollection<Question> candidates = await questions
            .ListSelectableAsync(context.CategoryId, context.AskedQuestionIds, cancellationToken)
            .ConfigureAwait(false);

        return candidates.Count == 0
            ? Result.Failure<Question>(QuestionErrors.NoneAvailable(context.CategoryId))
            : random.Pick([.. candidates]);
    }
}

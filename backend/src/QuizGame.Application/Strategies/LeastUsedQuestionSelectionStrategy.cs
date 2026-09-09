using QuizGame.Application.Abstractions.Persistence;
using QuizGame.Domain.Questions;
using QuizGame.Domain.SeedWork;

namespace QuizGame.Application.Strategies;

public sealed class LeastUsedQuestionSelectionStrategy(IQuestionRepository questions) : IQuestionSelectionStrategy
{
    public async Task<Result<Question>> SelectAsync(
        QuestionSelectionContext context,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(context);

        IReadOnlyCollection<Question> candidates = await questions
            .ListSelectableAsync(context.CategoryId, context.AskedQuestionIds, cancellationToken)
            .ConfigureAwait(false);

        if (candidates.Count == 0)
        {
            return Result.Failure<Question>(QuestionErrors.NoneAvailable(context.CategoryId));
        }

        IReadOnlyDictionary<Guid, int> usage = await questions
            .GetUsageCountsAsync([.. candidates.Select(question => question.Id)], cancellationToken)
            .ConfigureAwait(false);

        return candidates
            .OrderBy(question => usage.TryGetValue(question.Id, out int count) ? count : 0)
            .ThenBy(question => question.Id)
            .First();
    }
}

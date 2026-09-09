using QuizGame.Application.Abstractions;
using QuizGame.Application.Abstractions.Persistence;
using QuizGame.Domain.Questions;
using QuizGame.Domain.SeedWork;

namespace QuizGame.Application.Strategies;

public sealed class WeightedQuestionSelectionStrategy(
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

        if (candidates.Count == 0)
        {
            return Result.Failure<Question>(QuestionErrors.NoneAvailable(context.CategoryId));
        }

        // Later rounds bias the draw towards longer, denser statements.
        List<Question> pool = [];
        foreach (Question question in candidates)
        {
            int weight = 1 + (question.Text.Value.Length / 80 * context.RoundNumber.Value);
            for (int occurrence = 0; occurrence < weight; occurrence++)
            {
                pool.Add(question);
            }
        }

        return random.Pick(pool);
    }
}

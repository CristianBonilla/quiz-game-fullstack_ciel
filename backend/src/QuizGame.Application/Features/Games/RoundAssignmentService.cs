using Microsoft.Extensions.Options;
using QuizGame.Application.Abstractions;
using QuizGame.Application.Abstractions.Configuration;
using QuizGame.Application.Abstractions.Persistence;
using QuizGame.Application.Strategies;
using QuizGame.Domain.Categories;
using QuizGame.Domain.Games;
using QuizGame.Domain.Questions;
using QuizGame.Domain.SeedWork;
using QuizGame.Domain.ValueObjects;

namespace QuizGame.Application.Features.Games;

/// <summary>Orchestrates the plumbing a new round needs: category, question, prize and deadline.</summary>
public sealed class RoundAssignmentService(
    ICategoryRepository categories,
    IStrategyResolver<IQuestionSelectionStrategy> questionStrategies,
    IStrategyResolver<IPrizeCalculationStrategy> prizeStrategies,
    IOptions<GameOptions> options,
    IClock clock)
{
    private readonly GameOptions _options = options.Value;

    public async Task<Result<Question>> AssignNextRoundAsync(Game game, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(game);

        RoundNumber round = game.CurrentRound;

        Category? category = await categories
            .GetActiveByDifficultyAsync(RoundDifficultyMapper.MapToDifficultyLevel(round.Value), cancellationToken)
            .ConfigureAwait(false);

        if (category is null)
        {
            return Result.Failure<Question>(CategoryErrors.NotFoundForRound(round.Value));
        }

        IQuestionSelectionStrategy selection = questionStrategies.Resolve(
            ResolveKey(_options.QuestionSelectionStrategyByCategory, category, _options.QuestionSelectionStrategy),
            StrategyKeys.QuestionSelection.Random);

        Result<Question> question = await selection
            .SelectAsync(
                new QuestionSelectionContext(category.Id, game.AskedQuestionIds, round),
                cancellationToken)
            .ConfigureAwait(false);

        if (question.IsFailure)
        {
            return question;
        }

        IPrizeCalculationStrategy prizes = prizeStrategies.Resolve(
            ResolveKey(_options.PrizeCalculationStrategyByCategory, category, _options.PrizeCalculationStrategy),
            StrategyKeys.PrizeCalculation.Tiered);

        Prize prizeAtStake = prizes.CalculateFor(round, category);
        DateTime startedAtUtc = clock.UtcNow;

        Result assignment = game.AssignQuestion(
            question.Value,
            prizeAtStake,
            startedAtUtc,
            startedAtUtc.Add(game.Settings.QuestionTimeLimit));

        return assignment.IsFailure ? Result.Failure<Question>(assignment.Error) : question;
    }

    private static string ResolveKey(
        IDictionary<string, string> overrides,
        Category category,
        string defaultKey) =>
        overrides.TryGetValue(category.Name, out string? byName) ? byName : defaultKey;
}

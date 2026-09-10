using Microsoft.Extensions.Options;
using QuizGame.Application.Abstractions;
using QuizGame.Application.Abstractions.Configuration;
using QuizGame.Application.Abstractions.Persistence;
using QuizGame.Application.Features.Games;
using QuizGame.Application.Strategies;
using QuizGame.Domain.Categories;
using QuizGame.Domain.Questions;
using QuizGame.Domain.SeedWork;

namespace QuizGame.Application.UnitTests.Games;

/// <summary>
/// RoundAssignmentService is a concrete application service (not a domain entity), so wiring it
/// with substituted repositories/strategies is the correct way to exercise handlers that depend on it.
/// </summary>
internal static class RoundAssignmentServiceTestFactory
{
    public static RoundAssignmentService Create(
        Category category,
        Question nextQuestion,
        IClock clock)
    {
        ICategoryRepository categories = Substitute.For<ICategoryRepository>();
        categories.GetActiveByDifficultyAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Category?>(category));

        IQuestionSelectionStrategy selectionStrategy = Substitute.For<IQuestionSelectionStrategy>();
        selectionStrategy.SelectAsync(Arg.Any<QuestionSelectionContext>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success(nextQuestion)));

        IStrategyResolver<IQuestionSelectionStrategy> questionStrategies =
            Substitute.For<IStrategyResolver<IQuestionSelectionStrategy>>();
        questionStrategies.Resolve(Arg.Any<string>(), Arg.Any<string>()).Returns(selectionStrategy);

        IPrizeCalculationStrategy prizeStrategy = Substitute.For<IPrizeCalculationStrategy>();
        prizeStrategy.CalculateFor(Arg.Any<Domain.ValueObjects.RoundNumber>(), Arg.Any<Category>())
            .Returns(category.PrizeAmount);

        IStrategyResolver<IPrizeCalculationStrategy> prizeStrategies =
            Substitute.For<IStrategyResolver<IPrizeCalculationStrategy>>();
        prizeStrategies.Resolve(Arg.Any<string>(), Arg.Any<string>()).Returns(prizeStrategy);

        IOptions<GameOptions> options = Options.Create(new GameOptions());

        return new RoundAssignmentService(categories, questionStrategies, prizeStrategies, options, clock);
    }
}

using Microsoft.Extensions.Options;
using QuizGame.Application.Abstractions;
using QuizGame.Application.Abstractions.Configuration;
using QuizGame.Application.Abstractions.Persistence;
using QuizGame.Application.Features.Games;
using QuizGame.Application.Strategies;
using QuizGame.Domain.Categories;
using QuizGame.Domain.Games;
using QuizGame.Domain.Questions;
using QuizGame.Domain.SeedWork;
using QuizGame.TestUtilities;

namespace QuizGame.Application.UnitTests.Games;

public sealed class RoundAssignmentServiceTests
{
    [Fact]
    public async Task AssignNextRoundAsync_Should_ExcludeAlreadyAskedQuestions_When_SelectingTheNextQuestion()
    {
        // Arrange
        IClock clock = Substitute.For<IClock>();
        clock.UtcNow.Returns(TestClock.FixedUtcNow);

        Game game = GameBuilder.AGame().InRound(2).Build();
        Guid askedQuestionId = game.AskedQuestionIds.Single();

        Category category = CategoryBuilder.ACategory().WithQuestions(5).Activated().Build();
        Question nextQuestion = QuestionBuilder.AQuestion().WithCategoryId(category.Id).Build();

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

        RoundAssignmentService service = new(
            categories,
            questionStrategies,
            prizeStrategies,
            Options.Create(new GameOptions()),
            clock);

        // Act
        await service.AssignNextRoundAsync(game, CancellationToken.None);

        // Assert
        await selectionStrategy.Received(1).SelectAsync(
            Arg.Is<QuestionSelectionContext>(context => context.AskedQuestionIds.Contains(askedQuestionId)),
            Arg.Any<CancellationToken>());
    }
}

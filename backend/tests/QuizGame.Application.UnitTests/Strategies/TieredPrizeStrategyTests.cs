using QuizGame.Application.Strategies;
using QuizGame.Domain.ValueObjects;
using QuizGame.TestUtilities;

namespace QuizGame.Application.UnitTests.Strategies;

public sealed class TieredPrizeStrategyTests
{
    [Fact]
    public void CalculateFor_Should_DoubleThePrize_When_RoundIsFinal()
    {
        // Arrange
        var category = CategoryBuilder.ACategory().WithPrizeAmount(500m).WithQuestions(5).Activated().Build();
        RoundNumber finalRound = RoundNumber.Create(5, totalRounds: 5).Value;
        TieredPrizeStrategy strategy = new();

        // Act
        Prize result = strategy.CalculateFor(finalRound, category);

        // Assert
        result.Amount.ShouldBe(1000m);
    }

    [Fact]
    public void CalculateFor_Should_ReturnCategoryPrize_When_RoundIsNotFinal()
    {
        // Arrange
        var category = CategoryBuilder.ACategory().WithPrizeAmount(500m).WithQuestions(5).Activated().Build();
        RoundNumber earlyRound = RoundNumber.Create(2, totalRounds: 5).Value;
        TieredPrizeStrategy strategy = new();

        // Act
        Prize result = strategy.CalculateFor(earlyRound, category);

        // Assert
        result.Amount.ShouldBe(500m);
    }
}

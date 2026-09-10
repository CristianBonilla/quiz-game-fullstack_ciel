using QuizGame.Domain.SeedWork;
using QuizGame.Domain.ValueObjects;

namespace QuizGame.Domain.UnitTests.ValueObjects;

public sealed class PrizeTests
{
    [Fact]
    public void Create_Should_ReturnFailure_When_AmountIsNegative()
    {
        // Arrange
        // Act
        Result<Prize> result = Prize.Create(-1m);

        // Assert
        result.IsFailure.ShouldBeTrue();
    }

    [Fact]
    public void Create_Should_ReturnSuccess_When_AmountIsZero()
    {
        // Arrange
        // Act
        Result<Prize> result = Prize.Create(0m);

        // Assert
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public void Add_Should_ReturnSumOfBothAmounts_When_AddingTwoPrizes()
    {
        // Arrange
        Prize first = Prize.Create(100m).Value;
        Prize second = Prize.Create(250m).Value;

        // Act
        Prize sum = first.Add(second);

        // Assert
        sum.Amount.ShouldBe(350m);
    }

    [Fact]
    public void Equals_Should_ReturnTrue_When_AmountsAreEqual()
    {
        // Arrange
        Prize first = Prize.Create(100m).Value;
        Prize second = Prize.Create(100m).Value;

        // Act
        bool areEqual = first.Equals(second);

        // Assert
        areEqual.ShouldBeTrue();
    }

    [Fact]
    public void Equals_Should_ReturnFalse_When_ComparedToNull()
    {
        // Arrange
        Prize prize = Prize.Create(100m).Value;

        // Act
        bool areEqual = prize.Equals(null);

        // Assert
        areEqual.ShouldBeFalse();
    }

    [Fact]
    public void Equals_Should_ReturnFalse_When_ComparedToUnrelatedObject()
    {
        // Arrange
        Prize prize = Prize.Create(100m).Value;

        // Act
        bool areEqual = prize.Equals("not a prize");

        // Assert
        areEqual.ShouldBeFalse();
    }

    [Fact]
    public void EqualityOperator_Should_ReturnTrue_When_AmountsMatch()
    {
        // Arrange
        Prize first = Prize.Create(100m).Value;
        Prize second = Prize.Create(100m).Value;

        // Act & Assert
        (first == second).ShouldBeTrue();
        first.GetHashCode().ShouldBe(second.GetHashCode());
    }

    [Fact]
    public void ComparisonOperators_Should_OrderByAmount_When_ComparingTwoPrizes()
    {
        // Arrange
        Prize low = Prize.Create(50m).Value;
        Prize high = Prize.Create(200m).Value;

        // Act & Assert
        (low < high).ShouldBeTrue();
        (low <= high).ShouldBeTrue();
        (high > low).ShouldBeTrue();
        (high >= low).ShouldBeTrue();
        (low != high).ShouldBeTrue();
    }

    [Fact]
    public void Zero_Should_HaveAmountOfZero()
    {
        // Arrange
        // Act
        Prize zero = Prize.Zero;

        // Assert
        zero.Amount.ShouldBe(0m);
    }
}

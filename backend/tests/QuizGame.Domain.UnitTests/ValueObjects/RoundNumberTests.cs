using QuizGame.Domain.SeedWork;
using QuizGame.Domain.ValueObjects;

namespace QuizGame.Domain.UnitTests.ValueObjects;

public sealed class RoundNumberTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void Create_Should_ReturnFailure_When_ValueIsOutsideTotalRounds(int value)
    {
        // Arrange
        // Act
        Result<RoundNumber> result = RoundNumber.Create(value, totalRounds: 5);

        // Assert
        result.IsFailure.ShouldBeTrue();
    }

    [Fact]
    public void IsFinal_Should_ReturnTrue_When_ValueEqualsTotalRounds()
    {
        // Arrange
        RoundNumber round = RoundNumber.Create(5, totalRounds: 5).Value;

        // Act
        bool isFinal = round.IsFinal;

        // Assert
        isFinal.ShouldBeTrue();
    }

    [Fact]
    public void IsFinal_Should_ReturnFalse_When_ValueIsBeforeTotalRounds()
    {
        // Arrange
        RoundNumber round = RoundNumber.Create(4, totalRounds: 5).Value;

        // Act
        bool isFinal = round.IsFinal;

        // Assert
        isFinal.ShouldBeFalse();
    }

    [Fact]
    public void Next_Should_ReturnNextRoundNumber_When_NotFinal()
    {
        // Arrange
        RoundNumber round = RoundNumber.Create(3, totalRounds: 5).Value;

        // Act
        Result<RoundNumber> result = round.Next();

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Value.ShouldBe(4);
    }

    [Fact]
    public void Next_Should_ReturnFailure_When_RoundIsFinal()
    {
        // Arrange
        RoundNumber round = RoundNumber.Create(5, totalRounds: 5).Value;

        // Act
        Result<RoundNumber> result = round.Next();

        // Assert
        result.IsFailure.ShouldBeTrue();
    }

    [Fact]
    public void ComparisonOperators_Should_OrderByValue_When_ComparingTwoRounds()
    {
        // Arrange
        RoundNumber low = RoundNumber.Create(1, totalRounds: 5).Value;
        RoundNumber high = RoundNumber.Create(4, totalRounds: 5).Value;

        // Act & Assert
        (low < high).ShouldBeTrue();
        (low <= high).ShouldBeTrue();
        (high > low).ShouldBeTrue();
        (high >= low).ShouldBeTrue();
        (low != high).ShouldBeTrue();
    }

    [Fact]
    public void Equals_Should_ReturnFalse_When_ComparedToNull()
    {
        // Arrange
        RoundNumber round = RoundNumber.Create(1, totalRounds: 5).Value;

        // Act
        bool areEqual = round.Equals(null);

        // Assert
        areEqual.ShouldBeFalse();
    }

    [Fact]
    public void Equals_Should_ReturnTrueAndMatchHashCode_When_ValueAndTotalRoundsMatch()
    {
        // Arrange
        RoundNumber first = RoundNumber.Create(2, totalRounds: 5).Value;
        RoundNumber second = RoundNumber.Create(2, totalRounds: 5).Value;

        // Act & Assert
        first.Equals(second).ShouldBeTrue();
        (first == second).ShouldBeTrue();
        first.GetHashCode().ShouldBe(second.GetHashCode());
    }
}

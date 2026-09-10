using QuizGame.Domain.SeedWork;
using QuizGame.Domain.ValueObjects;

namespace QuizGame.Domain.UnitTests.ValueObjects;

public sealed class DifficultyLevelTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void Create_Should_ReturnFailure_When_ValueIsOutsideAllowedRange(int value)
    {
        // Arrange
        // Act
        Result<DifficultyLevel> result = DifficultyLevel.Create(value);

        // Assert
        result.IsFailure.ShouldBeTrue();
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    public void Create_Should_ReturnSuccess_When_ValueIsWithinAllowedRange(int value)
    {
        // Arrange
        // Act
        Result<DifficultyLevel> result = DifficultyLevel.Create(value);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Value.ShouldBe(value);
    }

    [Fact]
    public void ComparisonOperators_Should_OrderByValue_When_ComparingTwoLevels()
    {
        // Arrange
        DifficultyLevel low = DifficultyLevel.Create(1).Value;
        DifficultyLevel anotherLow = DifficultyLevel.Create(1).Value;
        DifficultyLevel high = DifficultyLevel.Create(5).Value;

        // Act & Assert
        (low < high).ShouldBeTrue();
        (low <= high).ShouldBeTrue();
        (high > low).ShouldBeTrue();
        (high >= low).ShouldBeTrue();
        (low == anotherLow).ShouldBeTrue();
        (low != high).ShouldBeTrue();
    }

    [Fact]
    public void Equals_Should_ReturnFalse_When_ComparedValueIsNull()
    {
        // Arrange
        DifficultyLevel level = DifficultyLevel.Create(2).Value;

        // Act
        bool areEqual = level.Equals(null);

        // Assert
        areEqual.ShouldBeFalse();
    }

    [Fact]
    public void Equals_Should_ReturnFalse_When_ComparedToUnrelatedObject()
    {
        // Arrange
        DifficultyLevel level = DifficultyLevel.Create(2).Value;

        // Act
        bool areEqual = level.Equals("not a difficulty level");

        // Assert
        areEqual.ShouldBeFalse();
    }

    [Fact]
    public void GetHashCode_Should_BeConsistent_When_ValuesAreEqual()
    {
        // Arrange
        DifficultyLevel first = DifficultyLevel.Create(3).Value;
        DifficultyLevel second = DifficultyLevel.Create(3).Value;

        // Act & Assert
        first.GetHashCode().ShouldBe(second.GetHashCode());
    }
}

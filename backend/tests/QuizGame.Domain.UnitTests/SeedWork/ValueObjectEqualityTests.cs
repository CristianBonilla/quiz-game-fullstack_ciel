using QuizGame.Domain.SeedWork;
using QuizGame.Domain.ValueObjects;

namespace QuizGame.Domain.UnitTests.SeedWork;

/// <summary>
/// Prize/RoundNumber/DifficultyLevel each declare their own operator==/!= overloads, so exercising
/// them through the derived type never invokes ValueObject's base operators. These tests call the
/// base operators directly by declaring the variables as ValueObject.
/// </summary>
public sealed class ValueObjectEqualityTests
{
    [Fact]
    public void BaseEqualityOperator_Should_ReturnTrue_When_UnderlyingValuesMatch()
    {
        // Arrange
        ValueObject first = Prize.Create(100m).Value;
        ValueObject second = Prize.Create(100m).Value;

        // Act & Assert
        (first == second).ShouldBeTrue();
        (first != second).ShouldBeFalse();
    }

    [Fact]
    public void BaseEqualityOperator_Should_ReturnFalse_When_UnderlyingValuesDiffer()
    {
        // Arrange
        ValueObject first = Prize.Create(100m).Value;
        ValueObject second = Prize.Create(200m).Value;

        // Act & Assert
        (first == second).ShouldBeFalse();
        (first != second).ShouldBeTrue();
    }
}

using QuizGame.Domain.Games;
using QuizGame.Domain.Games.Errors;
using QuizGame.Domain.Games.Events;
using QuizGame.Domain.SeedWork;
using QuizGame.TestUtilities;

namespace QuizGame.Domain.UnitTests.Games;

public sealed class GameForceEndTests
{
    [Fact]
    public void ForceEnd_Should_TransitionToForcedEnd_When_ReasonIsProvided()
    {
        // Arrange
        Game game = GameBuilder.AGame().WithOpenRound().Build();

        // Act
        Result result = game.ForceEnd("Round time expired", TestClock.FixedUtcNow);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        game.Status.ShouldBe(GameStatus.ForcedEnd);
    }

    [Fact]
    public void ForceEnd_Should_ResetAccumulatedPrizeToZero_When_GameIsForciblyEnded()
    {
        // Arrange
        Game game = GameBuilder.AGame().InRound(3).WithOpenRound().Build();

        // Act
        game.ForceEnd("Administrative cancellation", TestClock.FixedUtcNow);

        // Assert
        game.AccumulatedPrize.Amount.ShouldBe(0m);
    }

    [Fact]
    public void ForceEnd_Should_RaiseGameForciblyEndedDomainEvent_When_GameIsForciblyEnded()
    {
        // Arrange
        Game game = GameBuilder.AGame().WithOpenRound().Build();

        // Act
        game.ForceEnd("Round time expired", TestClock.FixedUtcNow);

        // Assert
        game.DomainEvents.ShouldContain(domainEvent => domainEvent is GameForciblyEndedDomainEvent);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ForceEnd_Should_ReturnFailure_When_ReasonIsEmpty(string? reason)
    {
        // Arrange
        Game game = GameBuilder.AGame().WithOpenRound().Build();

        // Act
        Result result = game.ForceEnd(reason, TestClock.FixedUtcNow);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(GameErrors.EmptyForcedEndReason);
    }

    [Fact]
    public void ForceEnd_Should_ReturnFailure_When_GameAlreadyEnded()
    {
        // Arrange
        Game game = GameBuilder.AGame().WithOpenRound().Build();
        game.ForceEnd("First cancellation", TestClock.FixedUtcNow);

        // Act
        Result result = game.ForceEnd("Second cancellation", TestClock.FixedUtcNow);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(GameErrors.NotInProgress);
    }
}

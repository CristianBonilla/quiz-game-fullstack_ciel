using QuizGame.Domain.Games;
using QuizGame.Domain.Games.Errors;
using QuizGame.Domain.Games.Events;
using QuizGame.Domain.SeedWork;
using QuizGame.TestUtilities;

namespace QuizGame.Domain.UnitTests.Games;

public sealed class GameWithdrawTests
{
    [Fact]
    public void Withdraw_Should_TransitionToWithdrawn_When_RoundIsOpenAndUnanswered()
    {
        // Arrange
        Game game = GameBuilder.AGame().WithOpenRound().Build();

        // Act
        Result result = game.Withdraw(TestClock.FixedUtcNow);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        game.Status.ShouldBe(GameStatus.Withdrawn);
    }

    [Fact]
    public void Withdraw_Should_ConserveAccumulatedPrize_When_PlayerWithdraws()
    {
        // Arrange
        Game game = GameBuilder.AGame().InRound(3).WithOpenRound().Build();
        decimal accumulatedBeforeWithdraw = game.AccumulatedPrize.Amount;

        // Act
        game.Withdraw(TestClock.FixedUtcNow);

        // Assert
        game.AccumulatedPrize.Amount.ShouldBe(accumulatedBeforeWithdraw);
    }

    [Fact]
    public void Withdraw_Should_RaiseGameWithdrawnDomainEvent_When_PlayerWithdraws()
    {
        // Arrange
        Game game = GameBuilder.AGame().WithOpenRound().Build();

        // Act
        game.Withdraw(TestClock.FixedUtcNow);

        // Assert
        game.DomainEvents.ShouldContain(domainEvent => domainEvent is GameWithdrawnDomainEvent);
    }

    [Fact]
    public void Withdraw_Should_ReturnFailure_When_CalledAfterRoundAlreadyAdvanced()
    {
        // Arrange
        Guid correctAnswerId = Guid.NewGuid();
        Game game = GameBuilder.AGame().WithOpenRound(correctAnswerId: correctAnswerId).Build();
        game.Answer(correctAnswerId, TestClock.FixedUtcNow);

        // Act
        Result result = game.Withdraw(TestClock.FixedUtcNow);

        // Assert
        result.IsFailure.ShouldBeTrue();
    }

    [Fact]
    public void Withdraw_Should_ReturnFailure_When_NoOpenRoundExists()
    {
        // Arrange
        Game game = GameBuilder.AGame().Build();

        // Act
        Result result = game.Withdraw(TestClock.FixedUtcNow);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(GameErrors.NoOpenRound);
    }

    [Fact]
    public void Withdraw_Should_ReturnFailure_When_GameAlreadyEnded()
    {
        // Arrange
        Game game = GameBuilder.AGame().WithOpenRound().Build();
        game.Withdraw(TestClock.FixedUtcNow);

        // Act
        Result result = game.Withdraw(TestClock.FixedUtcNow);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(GameErrors.NotInProgress);
    }
}

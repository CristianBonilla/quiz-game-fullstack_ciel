using QuizGame.Domain.Games;
using QuizGame.Domain.Games.Errors;
using QuizGame.Domain.Games.Events;
using QuizGame.Domain.Questions;
using QuizGame.Domain.SeedWork;
using QuizGame.Domain.ValueObjects;
using QuizGame.TestUtilities;

namespace QuizGame.Domain.UnitTests.Games;

public sealed class GameAnswerTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void Answer_Should_AccumulatePrizeAndAdvanceRound_When_AnswerIsCorrectAndRoundIsNotFinal(int round)
    {
        // Arrange
        Guid correctAnswerId = Guid.NewGuid();
        Game game = GameBuilder.AGame().InRound(round).WithOpenRound(correctAnswerId: correctAnswerId, prize: 100m).Build();
        decimal expectedAccumulated = game.AccumulatedPrize.Amount + 100m;

        // Act
        Result result = game.Answer(correctAnswerId, TestClock.FixedUtcNow);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        game.Status.ShouldBe(GameStatus.InProgress);
        game.CurrentRound.Value.ShouldBe(round + 1);
        game.AccumulatedPrize.Amount.ShouldBe(expectedAccumulated);
    }

    [Fact]
    public void Answer_Should_TransitionToWon_When_FinalRoundIsAnsweredCorrectly()
    {
        // Arrange
        Guid correctAnswerId = Guid.NewGuid();
        Game game = GameBuilder.AGame().InRound(5).WithOpenRound(correctAnswerId: correctAnswerId, prize: 1600m).Build();

        // Act
        Result result = game.Answer(correctAnswerId, TestClock.FixedUtcNow);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        game.Status.ShouldBe(GameStatus.Won);
    }

    [Fact]
    public void Answer_Should_ConserveAccumulatedPrize_When_FinalRoundIsAnsweredCorrectly()
    {
        // Arrange
        Guid correctAnswerId = Guid.NewGuid();
        Game game = GameBuilder.AGame().InRound(5).WithOpenRound(correctAnswerId: correctAnswerId, prize: 1600m).Build();
        decimal expectedAccumulated = game.AccumulatedPrize.Amount + 1600m;

        // Act
        game.Answer(correctAnswerId, TestClock.FixedUtcNow);

        // Assert
        game.AccumulatedPrize.Amount.ShouldBe(expectedAccumulated);
    }

    [Fact]
    public void Answer_Should_TransitionToLost_When_AnswerIsIncorrect()
    {
        // Arrange
        Guid correctAnswerId = Guid.NewGuid();
        Guid incorrectAnswerId = Guid.NewGuid();
        Game game = GameBuilder.AGame().InRound(3).WithOpenRound(correctAnswerId: correctAnswerId, prize: 400m).Build();

        // Act
        Result result = game.Answer(incorrectAnswerId, TestClock.FixedUtcNow);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        game.Status.ShouldBe(GameStatus.Lost);
    }

    [Fact]
    public void Answer_Should_ResetAccumulatedPrizeToZero_When_AnswerIsIncorrect()
    {
        // Arrange
        Guid correctAnswerId = Guid.NewGuid();
        Guid incorrectAnswerId = Guid.NewGuid();
        Game game = GameBuilder.AGame().InRound(3).WithOpenRound(correctAnswerId: correctAnswerId, prize: 400m).Build();

        // Act
        game.Answer(incorrectAnswerId, TestClock.FixedUtcNow);

        // Assert
        game.AccumulatedPrize.Amount.ShouldBe(0m);
    }

    [Fact]
    public void Answer_Should_RaiseAnswerSubmittedDomainEvent_When_AnswerIsSubmitted()
    {
        // Arrange
        Guid correctAnswerId = Guid.NewGuid();
        Game game = GameBuilder.AGame().WithOpenRound(correctAnswerId: correctAnswerId).Build();

        // Act
        game.Answer(correctAnswerId, TestClock.FixedUtcNow);

        // Assert
        game.DomainEvents.ShouldContain(domainEvent => domainEvent is AnswerSubmittedDomainEvent);
    }

    [Fact]
    public void Answer_Should_ReturnFailure_When_CalledAgainAfterRoundAlreadyAdvanced()
    {
        // Arrange
        Guid correctAnswerId = Guid.NewGuid();
        Game game = GameBuilder.AGame().WithOpenRound(correctAnswerId: correctAnswerId).Build();
        game.Answer(correctAnswerId, TestClock.FixedUtcNow);

        // Act
        Result result = game.Answer(correctAnswerId, TestClock.FixedUtcNow);

        // Assert
        result.IsFailure.ShouldBeTrue();
    }

    [Fact]
    public void Answer_Should_ReturnFailure_When_GameAlreadyEnded()
    {
        // Arrange
        Guid correctAnswerId = Guid.NewGuid();
        Guid incorrectAnswerId = Guid.NewGuid();
        Game game = GameBuilder.AGame().WithOpenRound(correctAnswerId: correctAnswerId).Build();
        game.Answer(incorrectAnswerId, TestClock.FixedUtcNow);

        // Act
        Result result = game.Answer(correctAnswerId, TestClock.FixedUtcNow);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(GameErrors.NotInProgress);
    }

    [Fact]
    public void Answer_Should_ReturnFailure_When_NoOpenRoundExists()
    {
        // Arrange
        Game game = GameBuilder.AGame().Build();

        // Act
        Result result = game.Answer(Guid.NewGuid(), TestClock.FixedUtcNow);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(GameErrors.NoOpenRound);
    }

    [Fact]
    public void Answer_Should_ReturnFailure_When_DeadlineHasExpired()
    {
        // Arrange
        Guid correctAnswerId = Guid.NewGuid();
        DateTime deadline = TestClock.FixedUtcNow.AddSeconds(30);
        Game game = GameBuilder.AGame().WithOpenRound(correctAnswerId: correctAnswerId, deadline: deadline).Build();

        // Act
        Result result = game.Answer(correctAnswerId, deadline.AddSeconds(1));

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(GameErrors.RoundExpired);
    }
}

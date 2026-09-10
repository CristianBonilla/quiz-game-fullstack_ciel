using QuizGame.Domain.Games;
using QuizGame.Domain.Games.Rules;
using QuizGame.Domain.ValueObjects;

namespace QuizGame.Domain.UnitTests.Games;

public sealed class GameRulesTests
{
    [Fact]
    public void EvaluateAnswer_Should_ReturnCorrect_When_SelectedAnswerMatchesCorrectAnswer()
    {
        // Arrange
        Guid answerId = Guid.NewGuid();

        // Act
        RoundOutcome outcome = GameRules.EvaluateAnswer(answerId, answerId);

        // Assert
        outcome.ShouldBe(RoundOutcome.Correct);
    }

    [Fact]
    public void EvaluateAnswer_Should_ReturnIncorrect_When_SelectedAnswerDoesNotMatch()
    {
        // Arrange
        // Act
        RoundOutcome outcome = GameRules.EvaluateAnswer(Guid.NewGuid(), Guid.NewGuid());

        // Assert
        outcome.ShouldBe(RoundOutcome.Incorrect);
    }

    [Fact]
    public void AccumulateFor_Should_AddAtStakeToAccumulated_When_OutcomeIsCorrect()
    {
        // Arrange
        Prize accumulated = Prize.Create(100m).Value;
        Prize atStake = Prize.Create(200m).Value;

        // Act
        Prize result = GameRules.AccumulateFor(RoundOutcome.Correct, accumulated, atStake);

        // Assert
        result.Amount.ShouldBe(300m);
    }

    [Fact]
    public void AccumulateFor_Should_ResetToZero_When_OutcomeIsIncorrect()
    {
        // Arrange
        Prize accumulated = Prize.Create(100m).Value;
        Prize atStake = Prize.Create(200m).Value;

        // Act
        Prize result = GameRules.AccumulateFor(RoundOutcome.Incorrect, accumulated, atStake);

        // Assert
        result.Amount.ShouldBe(0m);
    }

    [Fact]
    public void ResolveStatusAfterAnswer_Should_ReturnWon_When_CorrectAndFinalRound()
    {
        // Arrange
        // Act
        GameStatus status = GameRules.ResolveStatusAfterAnswer(RoundOutcome.Correct, isFinalRound: true);

        // Assert
        status.ShouldBe(GameStatus.Won);
    }

    [Fact]
    public void ResolveStatusAfterAnswer_Should_ReturnInProgress_When_CorrectAndNotFinalRound()
    {
        // Arrange
        // Act
        GameStatus status = GameRules.ResolveStatusAfterAnswer(RoundOutcome.Correct, isFinalRound: false);

        // Assert
        status.ShouldBe(GameStatus.InProgress);
    }

    [Fact]
    public void ResolveStatusAfterAnswer_Should_ReturnLost_When_Incorrect()
    {
        // Arrange
        // Act
        GameStatus status = GameRules.ResolveStatusAfterAnswer(RoundOutcome.Incorrect, isFinalRound: false);

        // Assert
        status.ShouldBe(GameStatus.Lost);
    }

    [Fact]
    public void AccumulateFor_Should_ReturnAccumulatedUnchanged_When_OutcomeIsPending()
    {
        // Arrange
        Prize accumulated = Prize.Create(100m).Value;
        Prize atStake = Prize.Create(200m).Value;

        // Act
        Prize result = GameRules.AccumulateFor(RoundOutcome.Pending, accumulated, atStake);

        // Assert
        result.ShouldBe(accumulated);
    }

    [Fact]
    public void ResolveStatusAfterAnswer_Should_ReturnInProgress_When_OutcomeIsPending()
    {
        // Arrange
        // Act
        GameStatus status = GameRules.ResolveStatusAfterAnswer(RoundOutcome.Pending, isFinalRound: false);

        // Assert
        status.ShouldBe(GameStatus.InProgress);
    }

    [Fact]
    public void HasExpired_Should_ReturnTrue_When_AnsweredAfterDeadline()
    {
        // Arrange
        DateTime deadline = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        DateTime answeredAt = deadline.AddSeconds(1);

        // Act
        bool expired = GameRules.HasExpired(deadline, answeredAt);

        // Assert
        expired.ShouldBeTrue();
    }

    [Fact]
    public void HasExpired_Should_ReturnFalse_When_AnsweredBeforeDeadline()
    {
        // Arrange
        DateTime deadline = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        DateTime answeredAt = deadline.AddSeconds(-1);

        // Act
        bool expired = GameRules.HasExpired(deadline, answeredAt);

        // Assert
        expired.ShouldBeFalse();
    }
}

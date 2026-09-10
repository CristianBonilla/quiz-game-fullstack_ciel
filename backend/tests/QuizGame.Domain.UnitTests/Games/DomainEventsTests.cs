using QuizGame.Domain.Games.Events;

namespace QuizGame.Domain.UnitTests.Games;

/// <summary>
/// Domain events are plain records raised by Game (see GameAnswerTests/GameWithdrawTests/etc.).
/// These tests exercise their generated Equals/GetHashCode/ToString members directly, since those
/// paths aren't hit by merely asserting the event type after a Game operation.
/// </summary>
public sealed class DomainEventsTests
{
    [Fact]
    public void GameStartedDomainEvent_Should_BeEqual_When_AllPropertiesMatch()
    {
        // Arrange
        Guid gameId = Guid.NewGuid();
        DateTime now = DateTime.UtcNow;
        GameStartedDomainEvent first = new(gameId, "Alice", 5, now);
        GameStartedDomainEvent second = new(gameId, "Alice", 5, now);

        // Act & Assert
        first.ShouldBe(second);
        first.GetHashCode().ShouldBe(second.GetHashCode());
        first.ToString().ShouldContain("Alice");
    }

    [Fact]
    public void RoundStartedDomainEvent_Should_ExposeConstructorValues()
    {
        // Arrange
        Guid gameId = Guid.NewGuid();
        Guid questionId = Guid.NewGuid();
        DateTime deadline = DateTime.UtcNow.AddSeconds(30);
        DateTime now = DateTime.UtcNow;

        // Act
        RoundStartedDomainEvent domainEvent = new(gameId, 1, questionId, 100m, deadline, now);

        // Assert
        domainEvent.GameId.ShouldBe(gameId);
        domainEvent.RoundNumber.ShouldBe(1);
        domainEvent.QuestionId.ShouldBe(questionId);
        domainEvent.PrizeAtStake.ShouldBe(100m);
        domainEvent.DeadlineUtc.ShouldBe(deadline);
        domainEvent.ToString().ShouldContain("RoundStartedDomainEvent");
    }

    [Fact]
    public void AnswerSubmittedDomainEvent_Should_ExposeConstructorValues()
    {
        // Arrange
        Guid gameId = Guid.NewGuid();
        Guid questionId = Guid.NewGuid();
        Guid answerId = Guid.NewGuid();
        DateTime now = DateTime.UtcNow;

        // Act
        AnswerSubmittedDomainEvent domainEvent = new(gameId, 1, questionId, answerId, IsCorrect: true, now);

        // Assert
        domainEvent.IsCorrect.ShouldBeTrue();
        domainEvent.SelectedAnswerId.ShouldBe(answerId);
        domainEvent.ToString().ShouldContain("AnswerSubmittedDomainEvent");
    }

    [Fact]
    public void PrizeAccumulatedDomainEvent_Should_ExposeConstructorValues()
    {
        // Arrange
        Guid gameId = Guid.NewGuid();
        DateTime now = DateTime.UtcNow;

        // Act
        PrizeAccumulatedDomainEvent domainEvent = new(gameId, 2, 200m, 300m, now);

        // Assert
        domainEvent.PrizeWon.ShouldBe(200m);
        domainEvent.AccumulatedPrize.ShouldBe(300m);
        domainEvent.ToString().ShouldContain("PrizeAccumulatedDomainEvent");
    }

    [Fact]
    public void RoundAdvancedDomainEvent_Should_ExposeConstructorValues()
    {
        // Arrange
        Guid gameId = Guid.NewGuid();
        DateTime now = DateTime.UtcNow;

        // Act
        RoundAdvancedDomainEvent domainEvent = new(gameId, 1, 2, 300m, now);

        // Assert
        domainEvent.PreviousRoundNumber.ShouldBe(1);
        domainEvent.CurrentRoundNumber.ShouldBe(2);
        domainEvent.ToString().ShouldContain("RoundAdvancedDomainEvent");
    }

    [Fact]
    public void GameWonDomainEvent_Should_ExposeConstructorValues()
    {
        // Arrange
        Guid gameId = Guid.NewGuid();
        DateTime now = DateTime.UtcNow;

        // Act
        GameWonDomainEvent domainEvent = new(gameId, "Alice", 1600m, now);

        // Assert
        domainEvent.FinalPrize.ShouldBe(1600m);
        domainEvent.ToString().ShouldContain("GameWonDomainEvent");
    }

    [Fact]
    public void GameLostDomainEvent_Should_ExposeConstructorValues()
    {
        // Arrange
        Guid gameId = Guid.NewGuid();
        DateTime now = DateTime.UtcNow;

        // Act
        GameLostDomainEvent domainEvent = new(gameId, "Alice", 3, 400m, now);

        // Assert
        domainEvent.ForfeitedPrize.ShouldBe(400m);
        domainEvent.ToString().ShouldContain("GameLostDomainEvent");
    }

    [Fact]
    public void GameWithdrawnDomainEvent_Should_ExposeConstructorValues()
    {
        // Arrange
        Guid gameId = Guid.NewGuid();
        DateTime now = DateTime.UtcNow;

        // Act
        GameWithdrawnDomainEvent domainEvent = new(gameId, "Alice", 2, 200m, now);

        // Assert
        domainEvent.AccumulatedPrize.ShouldBe(200m);
        domainEvent.ToString().ShouldContain("GameWithdrawnDomainEvent");
    }

    [Fact]
    public void GameForciblyEndedDomainEvent_Should_ExposeConstructorValues()
    {
        // Arrange
        Guid gameId = Guid.NewGuid();
        DateTime now = DateTime.UtcNow;

        // Act
        GameForciblyEndedDomainEvent domainEvent = new(gameId, "Alice", 2, "Timeout", now);

        // Assert
        domainEvent.Reason.ShouldBe("Timeout");
        domainEvent.ToString().ShouldContain("GameForciblyEndedDomainEvent");
    }
}

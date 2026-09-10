using QuizGame.Domain.Games;
using QuizGame.Domain.Games.Events;
using QuizGame.Domain.Questions;
using QuizGame.Domain.SeedWork;
using QuizGame.Domain.ValueObjects;
using QuizGame.TestUtilities;

namespace QuizGame.Domain.UnitTests.Games;

public sealed class GameCreationTests
{
    [Fact]
    public void Create_Should_ReturnInProgressGame_When_InputsAreValid()
    {
        // Arrange
        PlayerName playerName = PlayerName.Create("Alice").Value;
        GameSettings settings = GameSettings.Default;

        // Act
        Result<Game> result = Game.Create(Guid.NewGuid(), playerName, settings, TestClock.FixedUtcNow);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Status.ShouldBe(GameStatus.InProgress);
    }

    [Fact]
    public void Create_Should_SetCurrentRoundToFirst_When_GameIsCreated()
    {
        // Arrange
        PlayerName playerName = PlayerName.Create("Alice").Value;
        GameSettings settings = GameSettings.Default;

        // Act
        Result<Game> result = Game.Create(Guid.NewGuid(), playerName, settings, TestClock.FixedUtcNow);

        // Assert
        result.Value.CurrentRound.Value.ShouldBe(1);
    }

    [Fact]
    public void Create_Should_SetAccumulatedPrizeToZero_When_GameIsCreated()
    {
        // Arrange
        PlayerName playerName = PlayerName.Create("Alice").Value;
        GameSettings settings = GameSettings.Default;

        // Act
        Result<Game> result = Game.Create(Guid.NewGuid(), playerName, settings, TestClock.FixedUtcNow);

        // Assert
        result.Value.AccumulatedPrize.Amount.ShouldBe(0m);
    }

    [Fact]
    public void Create_Should_RaiseGameStartedDomainEvent_When_GameIsCreated()
    {
        // Arrange
        PlayerName playerName = PlayerName.Create("Alice").Value;
        GameSettings settings = GameSettings.Default;

        // Act
        Result<Game> result = Game.Create(Guid.NewGuid(), playerName, settings, TestClock.FixedUtcNow);

        // Assert
        result.Value.DomainEvents.ShouldContain(domainEvent => domainEvent is GameStartedDomainEvent);
    }

    [Fact]
    public void AssignQuestion_Should_RaiseRoundStartedDomainEvent_When_GameIsInProgress()
    {
        // Arrange
        Game game = GameBuilder.AGame().Build();
        Question question = QuestionBuilder.AQuestion().Build();

        // Act
        Result result = game.AssignQuestion(
            question,
            Prize.Create(100m).Value,
            TestClock.FixedUtcNow,
            TestClock.FixedUtcNow.AddSeconds(30));

        // Assert
        result.IsSuccess.ShouldBeTrue();
        game.DomainEvents.ShouldContain(domainEvent => domainEvent is RoundStartedDomainEvent);
    }

    [Fact]
    public void AssignQuestion_Should_ReturnFailure_When_RoundAlreadyOpen()
    {
        // Arrange
        Game game = GameBuilder.AGame().WithOpenRound().Build();
        Question anotherQuestion = QuestionBuilder.AQuestion().Build();

        // Act
        Result result = game.AssignQuestion(
            anotherQuestion,
            Prize.Create(100m).Value,
            TestClock.FixedUtcNow,
            TestClock.FixedUtcNow.AddSeconds(30));

        // Assert
        result.IsFailure.ShouldBeTrue();
    }

    [Fact]
    public void AssignQuestion_Should_ReturnFailure_When_QuestionWasAlreadyAskedInThisGame()
    {
        // Arrange
        Game game = GameBuilder.AGame().InRound(2).Build();
        Question repeatedQuestion = QuestionBuilder.AQuestion().WithId(game.AskedQuestionIds.First()).Build();

        // Act
        Result result = game.AssignQuestion(
            repeatedQuestion,
            Prize.Create(200m).Value,
            TestClock.FixedUtcNow,
            TestClock.FixedUtcNow.AddSeconds(30));

        // Assert
        result.IsFailure.ShouldBeTrue();
    }

    [Fact]
    public void AssignQuestion_Should_ReturnFailure_When_QuestionIsInactive()
    {
        // Arrange
        Game game = GameBuilder.AGame().Build();
        Question question = QuestionBuilder.AQuestion().Build();
        question.Deactivate();

        // Act
        Result result = game.AssignQuestion(
            question,
            Prize.Create(100m).Value,
            TestClock.FixedUtcNow,
            TestClock.FixedUtcNow.AddSeconds(30));

        // Assert
        result.IsFailure.ShouldBeTrue();
    }
}

using QuizGame.Domain.Games;
using QuizGame.Domain.Games.Rules;
using QuizGame.TestUtilities;

namespace QuizGame.Domain.UnitTests.Games;

public sealed class GameStateTransitionTests
{
    [Theory]
    [InlineData(GameStatus.NotStarted, GameStatus.InProgress, true)]
    [InlineData(GameStatus.InProgress, GameStatus.InProgress, true)]
    [InlineData(GameStatus.InProgress, GameStatus.Won, true)]
    [InlineData(GameStatus.InProgress, GameStatus.Lost, true)]
    [InlineData(GameStatus.InProgress, GameStatus.Withdrawn, true)]
    [InlineData(GameStatus.InProgress, GameStatus.ForcedEnd, true)]
    [InlineData(GameStatus.NotStarted, GameStatus.Won, false)]
    [InlineData(GameStatus.Won, GameStatus.InProgress, false)]
    [InlineData(GameStatus.Lost, GameStatus.InProgress, false)]
    [InlineData(GameStatus.Withdrawn, GameStatus.InProgress, false)]
    [InlineData(GameStatus.ForcedEnd, GameStatus.InProgress, false)]
    public void CanTransitionTo_Should_MatchDocumentedStateMachine_When_EvaluatingTransition(
        GameStatus from,
        GameStatus to,
        bool expected)
    {
        // Arrange
        // Act
        bool canTransition = GameStateMachine.CanTransitionTo(from, to);

        // Assert
        canTransition.ShouldBe(expected);
    }

    [Theory]
    [InlineData(GameStatus.Won)]
    [InlineData(GameStatus.Lost)]
    [InlineData(GameStatus.Withdrawn)]
    [InlineData(GameStatus.ForcedEnd)]
    public void IsTerminal_Should_ReturnTrue_When_StatusIsAnEndState(GameStatus status)
    {
        // Arrange
        // Act
        bool isTerminal = GameStateMachine.IsTerminal(status);

        // Assert
        isTerminal.ShouldBeTrue();
    }

    [Fact]
    public void IsTerminal_Should_ReturnFalse_When_StatusIsInProgress()
    {
        // Arrange
        // Act
        bool isTerminal = GameStateMachine.IsTerminal(GameStatus.InProgress);

        // Assert
        isTerminal.ShouldBeFalse();
    }

    [Fact]
    public void CanTransitionTo_Should_DelegateToStateMachine_When_CalledOnGameInstance()
    {
        // Arrange
        Game game = GameBuilder.AGame().Build();

        // Act
        bool canTransition = game.CanTransitionTo(GameStatus.Withdrawn);

        // Assert
        canTransition.ShouldBeTrue();
    }
}

using QuizGame.Domain.Games;
using QuizGame.Domain.SeedWork;
using QuizGame.Domain.ValueObjects;

namespace QuizGame.Domain.UnitTests.Games;

public sealed class GameSettingsTests
{
    [Fact]
    public void Create_Should_ReturnFailure_When_TotalRoundsIsZeroOrLess()
    {
        // Arrange
        // Act
        Result<GameSettings> result = GameSettings.Create(0, TimeSpan.FromSeconds(30), new Dictionary<int, Prize>());

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(GameSettingsErrors.InvalidTotalRounds);
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_QuestionTimeLimitIsZeroOrLess()
    {
        // Arrange
        Dictionary<int, Prize> prizes = new() { [1] = Prize.Create(100m).Value };

        // Act
        Result<GameSettings> result = GameSettings.Create(1, TimeSpan.Zero, prizes);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(GameSettingsErrors.InvalidQuestionTimeLimit);
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_APrizeIsMissingForARound()
    {
        // Arrange
        Dictionary<int, Prize> prizes = new() { [1] = Prize.Create(100m).Value };

        // Act
        Result<GameSettings> result = GameSettings.Create(2, TimeSpan.FromSeconds(30), prizes);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(GameSettingsErrors.MissingPrizeForRound(2));
    }

    [Fact]
    public void Create_Should_ReturnSuccess_When_AllRoundsHavePrizesConfigured()
    {
        // Arrange
        Dictionary<int, Prize> prizes = new()
        {
            [1] = Prize.Create(100m).Value,
            [2] = Prize.Create(200m).Value,
        };

        // Act
        Result<GameSettings> result = GameSettings.Create(2, TimeSpan.FromSeconds(30), prizes);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.TotalRounds.ShouldBe(2);
        result.Value.PrizeByRound.Count.ShouldBe(2);
    }

    [Fact]
    public void PrizeForRound_Should_ReturnFailure_When_RoundIsNotConfigured()
    {
        // Arrange
        GameSettings settings = GameSettings.Default;

        // Act
        Result<Prize> result = settings.PrizeForRound(99);

        // Assert
        result.IsFailure.ShouldBeTrue();
    }

    [Fact]
    public void Default_Should_ExposeFiveRoundsWithDoublingPrizes()
    {
        // Arrange
        // Act
        GameSettings settings = GameSettings.Default;

        // Assert
        settings.TotalRounds.ShouldBe(5);
        settings.PrizeForRound(1).Value.Amount.ShouldBe(100m);
        settings.PrizeForRound(5).Value.Amount.ShouldBe(1600m);
    }

    [Fact]
    public void Equals_Should_ReturnTrue_When_SameRoundsTimeLimitAndPrizesAreConfigured()
    {
        // Arrange
        Dictionary<int, Prize> prizes = new() { [1] = Prize.Create(100m).Value };
        GameSettings first = GameSettings.Create(1, TimeSpan.FromSeconds(30), prizes).Value;
        GameSettings second = GameSettings.Create(1, TimeSpan.FromSeconds(30), prizes).Value;

        // Act & Assert
        first.Equals(second).ShouldBeTrue();
        first.GetHashCode().ShouldBe(second.GetHashCode());
    }
}

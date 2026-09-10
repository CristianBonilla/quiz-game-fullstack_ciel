using QuizGame.Application.Abstractions;
using QuizGame.Application.Abstractions.Persistence;
using QuizGame.Application.Features.Games;
using QuizGame.Application.Features.Games.ForceEndGame;
using QuizGame.Domain.Games;
using QuizGame.Domain.Games.Errors;
using QuizGame.Domain.SeedWork;
using QuizGame.TestUtilities;

namespace QuizGame.Application.UnitTests.Games;

public sealed class ForceEndGameCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_Should_ReturnZeroAccumulatedPrize_When_GameIsForciblyEnded()
    {
        // Arrange
        Game game = GameBuilder.AGame().InRound(3).WithOpenRound().Build();

        IGameRepository games = Substitute.For<IGameRepository>();
        games.GetByIdAsync(game.Id, Arg.Any<CancellationToken>()).Returns(Task.FromResult<Game?>(game));

        IClock clock = Substitute.For<IClock>();
        clock.UtcNow.Returns(TestClock.FixedUtcNow);

        ForceEndGameCommandHandler handler = new(games, clock);

        // Act
        Result<GameSummaryResponse> result = await handler.HandleAsync(
            new ForceEndGameCommand(game.Id, "Round time expired"),
            CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Status.ShouldBe(nameof(GameStatus.ForcedEnd));
        result.Value.FinalPrize.ShouldBe(0m);
    }

    [Fact]
    public async Task HandleAsync_Should_ReturnFailure_When_GameDoesNotExist()
    {
        // Arrange
        Guid gameId = Guid.NewGuid();
        IGameRepository games = Substitute.For<IGameRepository>();
        games.GetByIdAsync(gameId, Arg.Any<CancellationToken>()).Returns(Task.FromResult<Game?>(null));

        IClock clock = Substitute.For<IClock>();

        ForceEndGameCommandHandler handler = new(games, clock);

        // Act
        Result<GameSummaryResponse> result = await handler.HandleAsync(
            new ForceEndGameCommand(gameId, "Round time expired"),
            CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(GameErrors.NotFound(gameId));
    }
}

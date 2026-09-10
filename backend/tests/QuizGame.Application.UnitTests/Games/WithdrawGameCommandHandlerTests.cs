using QuizGame.Application.Abstractions;
using QuizGame.Application.Abstractions.Persistence;
using QuizGame.Application.Features.Games;
using QuizGame.Application.Features.Games.WithdrawGame;
using QuizGame.Domain.Games;
using QuizGame.Domain.Games.Errors;
using QuizGame.Domain.SeedWork;
using QuizGame.TestUtilities;

namespace QuizGame.Application.UnitTests.Games;

public sealed class WithdrawGameCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_Should_ReturnSummaryWithConservedPrize_When_RoundIsOpenAndUnanswered()
    {
        // Arrange
        Game game = GameBuilder.AGame().InRound(3).WithOpenRound().Build();
        decimal accumulatedBeforeWithdraw = game.AccumulatedPrize.Amount;

        IGameRepository games = Substitute.For<IGameRepository>();
        games.GetByIdAsync(game.Id, Arg.Any<CancellationToken>()).Returns(Task.FromResult<Game?>(game));

        IClock clock = Substitute.For<IClock>();
        clock.UtcNow.Returns(TestClock.FixedUtcNow);

        WithdrawGameCommandHandler handler = new(games, clock);

        // Act
        Result<GameSummaryResponse> result = await handler.HandleAsync(
            new WithdrawGameCommand(game.Id),
            CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Status.ShouldBe(nameof(GameStatus.Withdrawn));
        result.Value.FinalPrize.ShouldBe(accumulatedBeforeWithdraw);
    }

    [Fact]
    public async Task HandleAsync_Should_ReturnFailure_When_GameDoesNotExist()
    {
        // Arrange
        Guid gameId = Guid.NewGuid();
        IGameRepository games = Substitute.For<IGameRepository>();
        games.GetByIdAsync(gameId, Arg.Any<CancellationToken>()).Returns(Task.FromResult<Game?>(null));

        IClock clock = Substitute.For<IClock>();

        WithdrawGameCommandHandler handler = new(games, clock);

        // Act
        Result<GameSummaryResponse> result = await handler.HandleAsync(
            new WithdrawGameCommand(gameId),
            CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(GameErrors.NotFound(gameId));
    }
}

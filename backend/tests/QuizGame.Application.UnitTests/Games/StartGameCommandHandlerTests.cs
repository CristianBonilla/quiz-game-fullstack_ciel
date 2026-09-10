using Microsoft.Extensions.Options;
using QuizGame.Application.Abstractions;
using QuizGame.Application.Abstractions.Configuration;
using QuizGame.Application.Abstractions.Persistence;
using QuizGame.Application.Features.Games;
using QuizGame.Application.Features.Games.StartGame;
using QuizGame.Domain.Categories;
using QuizGame.Domain.Games;
using QuizGame.Domain.Questions;
using QuizGame.Domain.SeedWork;
using QuizGame.TestUtilities;

namespace QuizGame.Application.UnitTests.Games;

public sealed class StartGameCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_Should_ReturnGameStateResponse_When_PlayerNameIsValid()
    {
        // Arrange
        IGameRepository games = Substitute.For<IGameRepository>();
        IClock clock = Substitute.For<IClock>();
        clock.UtcNow.Returns(TestClock.FixedUtcNow);

        Category category = CategoryBuilder.ACategory().WithQuestions(5).Activated().Build();
        Question nextQuestion = QuestionBuilder.AQuestion().WithCategoryId(category.Id).Build();

        StartGameCommandHandler handler = new(
            games,
            new GameSettingsFactory(Options.Create(new GameOptions())),
            RoundAssignmentServiceTestFactory.Create(category, nextQuestion, clock),
            clock);

        // Act
        Result<GameStateResponse> result = await handler.HandleAsync(new StartGameCommand("Alice"), CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Status.ShouldBe(nameof(GameStatus.InProgress));
        result.Value.CurrentRound.ShouldBe(1);
    }

    [Fact]
    public async Task HandleAsync_Should_AddGameToRepository_When_CreationSucceeds()
    {
        // Arrange
        IGameRepository games = Substitute.For<IGameRepository>();
        IClock clock = Substitute.For<IClock>();
        clock.UtcNow.Returns(TestClock.FixedUtcNow);

        Category category = CategoryBuilder.ACategory().WithQuestions(5).Activated().Build();
        Question nextQuestion = QuestionBuilder.AQuestion().WithCategoryId(category.Id).Build();

        StartGameCommandHandler handler = new(
            games,
            new GameSettingsFactory(Options.Create(new GameOptions())),
            RoundAssignmentServiceTestFactory.Create(category, nextQuestion, clock),
            clock);

        // Act
        await handler.HandleAsync(new StartGameCommand("Alice"), CancellationToken.None);

        // Assert
        games.Received(1).Add(Arg.Any<Game>());
    }

    [Fact]
    public async Task HandleAsync_Should_ReturnFailure_When_PlayerNameIsEmpty()
    {
        // Arrange
        IGameRepository games = Substitute.For<IGameRepository>();
        IClock clock = Substitute.For<IClock>();
        clock.UtcNow.Returns(TestClock.FixedUtcNow);

        Category category = CategoryBuilder.ACategory().WithQuestions(5).Activated().Build();
        Question nextQuestion = QuestionBuilder.AQuestion().WithCategoryId(category.Id).Build();

        StartGameCommandHandler handler = new(
            games,
            new GameSettingsFactory(Options.Create(new GameOptions())),
            RoundAssignmentServiceTestFactory.Create(category, nextQuestion, clock),
            clock);

        // Act
        Result<GameStateResponse> result = await handler.HandleAsync(new StartGameCommand(string.Empty), CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        games.DidNotReceive().Add(Arg.Any<Game>());
    }
}

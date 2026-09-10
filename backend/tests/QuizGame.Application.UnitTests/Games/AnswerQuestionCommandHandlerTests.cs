using QuizGame.Application.Abstractions;
using QuizGame.Application.Abstractions.Persistence;
using QuizGame.Application.Features.Games;
using QuizGame.Application.Features.Games.AnswerQuestion;
using QuizGame.Domain.Categories;
using QuizGame.Domain.Games;
using QuizGame.Domain.Questions;
using QuizGame.Domain.SeedWork;
using QuizGame.TestUtilities;

namespace QuizGame.Application.UnitTests.Games;

public sealed class AnswerQuestionCommandHandlerTests
{
    [Fact]
    public async Task HandleAsync_Should_AssignNextRound_When_AnswerIsCorrectAndGameContinues()
    {
        // Arrange
        Guid correctAnswerId = Guid.NewGuid();
        Guid questionId = Guid.NewGuid();
        Game game = GameBuilder.AGame()
            .InRound(1)
            .WithOpenRound(questionId: questionId, correctAnswerId: correctAnswerId, prize: 100m)
            .Build();

        Question askedQuestion = QuestionBuilder.AQuestion()
            .WithId(questionId)
            .WithCorrectAnswerAt(0, correctAnswerId)
            .Build();

        IGameRepository games = Substitute.For<IGameRepository>();
        games.GetByIdAsync(game.Id, Arg.Any<CancellationToken>()).Returns(Task.FromResult<Game?>(game));

        IQuestionRepository questions = Substitute.For<IQuestionRepository>();
        questions.GetByIdAsync(questionId, Arg.Any<CancellationToken>()).Returns(Task.FromResult<Question?>(askedQuestion));

        IClock clock = Substitute.For<IClock>();
        clock.UtcNow.Returns(TestClock.FixedUtcNow);

        Category category = CategoryBuilder.ACategory().WithQuestions(5).Activated().Build();
        Question nextQuestion = QuestionBuilder.AQuestion().WithCategoryId(category.Id).Build();

        AnswerQuestionCommandHandler handler = new(
            games,
            questions,
            RoundAssignmentServiceTestFactory.Create(category, nextQuestion, clock),
            clock);

        // Act
        Result<AnswerQuestionResponse> result = await handler.HandleAsync(
            new AnswerQuestionCommand(game.Id, correctAnswerId, Guid.NewGuid()),
            CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.IsCorrect.ShouldBeTrue();
        result.Value.CurrentRound.ShouldBe(2);
        result.Value.AccumulatedPrize.ShouldBe(100m);
        result.Value.NextQuestion.ShouldNotBeNull();
    }

    [Fact]
    public async Task HandleAsync_Should_NotExposeCorrectAnswer_When_NextQuestionIsAssigned()
    {
        // Arrange
        Guid correctAnswerId = Guid.NewGuid();
        Guid questionId = Guid.NewGuid();
        Game game = GameBuilder.AGame()
            .InRound(1)
            .WithOpenRound(questionId: questionId, correctAnswerId: correctAnswerId, prize: 100m)
            .Build();

        Question askedQuestion = QuestionBuilder.AQuestion()
            .WithId(questionId)
            .WithCorrectAnswerAt(0, correctAnswerId)
            .Build();

        IGameRepository games = Substitute.For<IGameRepository>();
        games.GetByIdAsync(game.Id, Arg.Any<CancellationToken>()).Returns(Task.FromResult<Game?>(game));

        IQuestionRepository questions = Substitute.For<IQuestionRepository>();
        questions.GetByIdAsync(questionId, Arg.Any<CancellationToken>()).Returns(Task.FromResult<Question?>(askedQuestion));

        IClock clock = Substitute.For<IClock>();
        clock.UtcNow.Returns(TestClock.FixedUtcNow);

        Category category = CategoryBuilder.ACategory().WithQuestions(5).Activated().Build();
        Question nextQuestion = QuestionBuilder.AQuestion().WithCategoryId(category.Id).Build();

        AnswerQuestionCommandHandler handler = new(
            games,
            questions,
            RoundAssignmentServiceTestFactory.Create(category, nextQuestion, clock),
            clock);

        // Act
        Result<AnswerQuestionResponse> result = await handler.HandleAsync(
            new AnswerQuestionCommand(game.Id, correctAnswerId, Guid.NewGuid()),
            CancellationToken.None);

        // Assert
        result.Value.NextQuestion!.Answers.Count.ShouldBe(Question.RequiredAnswers);
    }

    [Fact]
    public async Task HandleAsync_Should_ReturnLostStatusAndZeroPrize_When_AnswerIsIncorrect()
    {
        // Arrange
        Guid questionId = Guid.NewGuid();
        Question askedQuestion = QuestionBuilder.AQuestion()
            .WithId(questionId)
            .WithAnswers(("Correct", true), ("Wrong A", false), ("Wrong B", false), ("Wrong C", false))
            .Build();

        Guid correctAnswerId = askedQuestion.CorrectAnswerId;
        Guid incorrectAnswerId = askedQuestion.Answers.First(answer => !answer.IsCorrect).Id;

        Game game = GameBuilder.AGame()
            .InRound(3)
            .WithOpenRound(questionId: questionId, correctAnswerId: correctAnswerId, prize: 400m)
            .Build();

        IGameRepository games = Substitute.For<IGameRepository>();
        games.GetByIdAsync(game.Id, Arg.Any<CancellationToken>()).Returns(Task.FromResult<Game?>(game));

        IQuestionRepository questions = Substitute.For<IQuestionRepository>();
        questions.GetByIdAsync(questionId, Arg.Any<CancellationToken>()).Returns(Task.FromResult<Question?>(askedQuestion));

        IClock clock = Substitute.For<IClock>();
        clock.UtcNow.Returns(TestClock.FixedUtcNow);

        Category category = CategoryBuilder.ACategory().WithQuestions(5).Activated().Build();
        Question nextQuestion = QuestionBuilder.AQuestion().WithCategoryId(category.Id).Build();

        AnswerQuestionCommandHandler handler = new(
            games,
            questions,
            RoundAssignmentServiceTestFactory.Create(category, nextQuestion, clock),
            clock);

        // Act
        Result<AnswerQuestionResponse> result = await handler.HandleAsync(
            new AnswerQuestionCommand(game.Id, incorrectAnswerId, Guid.NewGuid()),
            CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.IsCorrect.ShouldBeFalse();
        result.Value.Status.ShouldBe(nameof(GameStatus.Lost));
        result.Value.AccumulatedPrize.ShouldBe(0m);
        result.Value.NextQuestion.ShouldBeNull();
    }

    [Fact]
    public async Task HandleAsync_Should_ReturnFailure_When_GameDoesNotExist()
    {
        // Arrange
        Guid gameId = Guid.NewGuid();
        IGameRepository games = Substitute.For<IGameRepository>();
        games.GetByIdAsync(gameId, Arg.Any<CancellationToken>()).Returns(Task.FromResult<Game?>(null));

        IQuestionRepository questions = Substitute.For<IQuestionRepository>();
        IClock clock = Substitute.For<IClock>();

        Category category = CategoryBuilder.ACategory().WithQuestions(5).Activated().Build();
        Question nextQuestion = QuestionBuilder.AQuestion().WithCategoryId(category.Id).Build();

        AnswerQuestionCommandHandler handler = new(
            games,
            questions,
            RoundAssignmentServiceTestFactory.Create(category, nextQuestion, clock),
            clock);

        // Act
        Result<AnswerQuestionResponse> result = await handler.HandleAsync(
            new AnswerQuestionCommand(gameId, Guid.NewGuid(), Guid.NewGuid()),
            CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        await questions.DidNotReceive().GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task HandleAsync_Should_ReturnFailure_When_AnswerDoesNotBelongToAskedQuestion()
    {
        // Arrange
        Guid correctAnswerId = Guid.NewGuid();
        Guid questionId = Guid.NewGuid();
        Game game = GameBuilder.AGame()
            .WithOpenRound(questionId: questionId, correctAnswerId: correctAnswerId)
            .Build();

        Question askedQuestion = QuestionBuilder.AQuestion()
            .WithId(questionId)
            .WithCorrectAnswerAt(0, correctAnswerId)
            .Build();

        IGameRepository games = Substitute.For<IGameRepository>();
        games.GetByIdAsync(game.Id, Arg.Any<CancellationToken>()).Returns(Task.FromResult<Game?>(game));

        IQuestionRepository questions = Substitute.For<IQuestionRepository>();
        questions.GetByIdAsync(questionId, Arg.Any<CancellationToken>()).Returns(Task.FromResult<Question?>(askedQuestion));

        IClock clock = Substitute.For<IClock>();
        clock.UtcNow.Returns(TestClock.FixedUtcNow);

        Category category = CategoryBuilder.ACategory().WithQuestions(5).Activated().Build();
        Question nextQuestion = QuestionBuilder.AQuestion().WithCategoryId(category.Id).Build();

        AnswerQuestionCommandHandler handler = new(
            games,
            questions,
            RoundAssignmentServiceTestFactory.Create(category, nextQuestion, clock),
            clock);

        // Act
        Result<AnswerQuestionResponse> result = await handler.HandleAsync(
            new AnswerQuestionCommand(game.Id, Guid.NewGuid(), Guid.NewGuid()),
            CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
    }
}

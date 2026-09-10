using QuizGame.Application.Abstractions;
using QuizGame.Application.Abstractions.Persistence;
using QuizGame.Application.Strategies;
using QuizGame.Domain.Questions;
using QuizGame.Domain.SeedWork;
using QuizGame.TestUtilities;

namespace QuizGame.Application.UnitTests.Strategies;

public sealed class RandomQuestionSelectionStrategyTests
{
    [Fact]
    public async Task SelectAsync_Should_ReturnQuestionFromRandomProvider_When_CandidatesExist()
    {
        // Arrange
        Guid categoryId = Guid.NewGuid();
        Question expected = QuestionBuilder.AQuestion().WithCategoryId(categoryId).Build();

        IQuestionRepository questions = Substitute.For<IQuestionRepository>();
        questions.ListSelectableAsync(categoryId, Arg.Any<IReadOnlyCollection<Guid>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyCollection<Question>>([expected]));

        IRandomProvider random = Substitute.For<IRandomProvider>();
        random.Pick(Arg.Any<IReadOnlyList<Question>>()).Returns(expected);

        RandomQuestionSelectionStrategy strategy = new(questions, random);
        QuestionSelectionContext context = new(categoryId, [], Domain.ValueObjects.RoundNumber.Create(1, totalRounds: 5).Value);

        // Act
        Result<Question> result = await strategy.SelectAsync(context, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(expected);
    }

    [Fact]
    public async Task SelectAsync_Should_ReturnFailure_When_NoCandidatesAvailable()
    {
        // Arrange
        Guid categoryId = Guid.NewGuid();

        IQuestionRepository questions = Substitute.For<IQuestionRepository>();
        questions.ListSelectableAsync(categoryId, Arg.Any<IReadOnlyCollection<Guid>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyCollection<Question>>([]));

        IRandomProvider random = Substitute.For<IRandomProvider>();

        RandomQuestionSelectionStrategy strategy = new(questions, random);
        QuestionSelectionContext context = new(categoryId, [], Domain.ValueObjects.RoundNumber.Create(1, totalRounds: 5).Value);

        // Act
        Result<Question> result = await strategy.SelectAsync(context, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        random.DidNotReceive().Pick(Arg.Any<IReadOnlyList<Question>>());
    }
}

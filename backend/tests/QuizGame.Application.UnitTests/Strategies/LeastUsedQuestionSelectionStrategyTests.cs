using QuizGame.Application.Abstractions.Persistence;
using QuizGame.Application.Strategies;
using QuizGame.Domain.Questions;
using QuizGame.Domain.SeedWork;
using QuizGame.TestUtilities;

namespace QuizGame.Application.UnitTests.Strategies;

public sealed class LeastUsedQuestionSelectionStrategyTests
{
    [Fact]
    public async Task SelectAsync_Should_ReturnLeastUsedQuestion_When_UsageCountsDiffer()
    {
        // Arrange
        Guid categoryId = Guid.NewGuid();
        Question mostUsed = QuestionBuilder.AQuestion().WithCategoryId(categoryId).Build();
        Question leastUsed = QuestionBuilder.AQuestion().WithCategoryId(categoryId).Build();

        IQuestionRepository questions = Substitute.For<IQuestionRepository>();
        questions.ListSelectableAsync(categoryId, Arg.Any<IReadOnlyCollection<Guid>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyCollection<Question>>([mostUsed, leastUsed]));
        questions.GetUsageCountsAsync(Arg.Any<IReadOnlyCollection<Guid>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyDictionary<Guid, int>>(
                new Dictionary<Guid, int> { [mostUsed.Id] = 10, [leastUsed.Id] = 1 }));

        LeastUsedQuestionSelectionStrategy strategy = new(questions);
        QuestionSelectionContext context = new(categoryId, [], Domain.ValueObjects.RoundNumber.Create(1, totalRounds: 5).Value);

        // Act
        Result<Question> result = await strategy.SelectAsync(context, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(leastUsed);
    }

    [Fact]
    public async Task SelectAsync_Should_ReturnFailure_When_NoCandidatesAvailable()
    {
        // Arrange
        Guid categoryId = Guid.NewGuid();

        IQuestionRepository questions = Substitute.For<IQuestionRepository>();
        questions.ListSelectableAsync(categoryId, Arg.Any<IReadOnlyCollection<Guid>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyCollection<Question>>([]));

        LeastUsedQuestionSelectionStrategy strategy = new(questions);
        QuestionSelectionContext context = new(categoryId, [], Domain.ValueObjects.RoundNumber.Create(1, totalRounds: 5).Value);

        // Act
        Result<Question> result = await strategy.SelectAsync(context, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
    }
}

using QuizGame.Domain.Questions;
using QuizGame.Domain.SeedWork;
using QuizGame.Domain.ValueObjects;
using QuizGame.TestUtilities;

namespace QuizGame.Domain.UnitTests.Questions;

public sealed class QuestionCreationTests
{
    [Fact]
    public void Create_Should_ReturnSuccess_When_ExactlyFourAnswersAndOneCorrect()
    {
        // Arrange
        List<AnswerCandidate> answers =
        [
            new(Guid.NewGuid(), AnswerText.Create("A").Value, true),
            new(Guid.NewGuid(), AnswerText.Create("B").Value, false),
            new(Guid.NewGuid(), AnswerText.Create("C").Value, false),
            new(Guid.NewGuid(), AnswerText.Create("D").Value, false)
        ];

        // Act
        Result<Question> result = Question.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            QuestionText.Create("What?").Value,
            answers);

        // Assert
        result.IsSuccess.ShouldBeTrue();
    }

    [Theory]
    [InlineData(3)]
    [InlineData(5)]
    public void Create_Should_ReturnFailure_When_AnswerCountIsNotFour(int answerCount)
    {
        // Arrange
        List<AnswerCandidate> answers = [.. Enumerable.Range(0, answerCount)
            .Select(index => new AnswerCandidate(Guid.NewGuid(), AnswerText.Create($"Option {index}").Value, index == 0))];

        // Act
        Result<Question> result = Question.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            QuestionText.Create("What?").Value,
            answers);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(QuestionErrors.InvalidAnswerCount);
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_NoAnswerIsCorrect()
    {
        // Arrange
        List<AnswerCandidate> answers =
        [
            new(Guid.NewGuid(), AnswerText.Create("A").Value, false),
            new(Guid.NewGuid(), AnswerText.Create("B").Value, false),
            new(Guid.NewGuid(), AnswerText.Create("C").Value, false),
            new(Guid.NewGuid(), AnswerText.Create("D").Value, false)
        ];

        // Act
        Result<Question> result = Question.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            QuestionText.Create("What?").Value,
            answers);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(QuestionErrors.InvalidCorrectAnswerCount);
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_MoreThanOneAnswerIsCorrect()
    {
        // Arrange
        List<AnswerCandidate> answers =
        [
            new(Guid.NewGuid(), AnswerText.Create("A").Value, true),
            new(Guid.NewGuid(), AnswerText.Create("B").Value, true),
            new(Guid.NewGuid(), AnswerText.Create("C").Value, false),
            new(Guid.NewGuid(), AnswerText.Create("D").Value, false)
        ];

        // Act
        Result<Question> result = Question.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            QuestionText.Create("What?").Value,
            answers);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(QuestionErrors.InvalidCorrectAnswerCount);
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_CategoryIdIsEmpty()
    {
        // Arrange
        List<AnswerCandidate> answers =
        [
            new(Guid.NewGuid(), AnswerText.Create("A").Value, true),
            new(Guid.NewGuid(), AnswerText.Create("B").Value, false),
            new(Guid.NewGuid(), AnswerText.Create("C").Value, false),
            new(Guid.NewGuid(), AnswerText.Create("D").Value, false)
        ];

        // Act
        Result<Question> result = Question.Create(
            Guid.NewGuid(),
            Guid.Empty,
            QuestionText.Create("What?").Value,
            answers);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(QuestionErrors.EmptyCategory);
    }

    [Fact]
    public void IsCorrectAnswer_Should_ReturnTrue_When_AnswerIdMatchesCorrectAnswer()
    {
        // Arrange
        Question question = QuestionBuilder.AQuestion().WithCorrectAnswerAt(2).Build();
        Guid correctAnswerId = question.CorrectAnswerId;

        // Act
        bool isCorrect = question.IsCorrectAnswer(correctAnswerId);

        // Assert
        isCorrect.ShouldBeTrue();
    }

    [Fact]
    public void IsCorrectAnswer_Should_ReturnFalse_When_AnswerIdDoesNotMatch()
    {
        // Arrange
        Question question = QuestionBuilder.AQuestion().WithCorrectAnswerAt(0).Build();

        // Act
        bool isCorrect = question.IsCorrectAnswer(Guid.NewGuid());

        // Assert
        isCorrect.ShouldBeFalse();
    }
}

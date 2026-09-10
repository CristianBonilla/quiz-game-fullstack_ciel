using QuizGame.Domain.Questions;
using QuizGame.Domain.SeedWork;
using QuizGame.Domain.ValueObjects;
using QuizGame.TestUtilities;

namespace QuizGame.Domain.UnitTests.Questions;

public sealed class QuestionManagementTests
{
    [Fact]
    public void Update_Should_ReplaceTextAndAnswers_When_InputIsValid()
    {
        // Arrange
        Question question = QuestionBuilder.AQuestion().Build();
        QuestionText newText = QuestionText.Create("What is the capital of France?").Value;
        List<AnswerCandidate> newAnswers =
        [
            Candidate("Paris", true),
            Candidate("Berlin", false),
            Candidate("Madrid", false),
            Candidate("Lisbon", false),
        ];

        // Act
        Result result = question.Update(newText, newAnswers);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        question.Text.ShouldBe(newText);
        question.Answers.Count.ShouldBe(4);
    }

    [Fact]
    public void Update_Should_ReturnFailure_When_AnswerCountIsNotFour()
    {
        // Arrange
        Question question = QuestionBuilder.AQuestion().Build();
        List<AnswerCandidate> invalidAnswers =
        [
            Candidate("Only one", true),
        ];

        // Act
        Result result = question.Update(QuestionText.Create("Text?").Value, invalidAnswers);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(QuestionErrors.InvalidAnswerCount);
    }

    [Fact]
    public void Update_Should_ReturnFailure_When_ThereIsNotExactlyOneCorrectAnswer()
    {
        // Arrange
        Question question = QuestionBuilder.AQuestion().Build();
        List<AnswerCandidate> invalidAnswers =
        [
            Candidate("A", true),
            Candidate("B", true),
            Candidate("C", false),
            Candidate("D", false),
        ];

        // Act
        Result result = question.Update(QuestionText.Create("Text?").Value, invalidAnswers);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(QuestionErrors.InvalidCorrectAnswerCount);
    }

    [Fact]
    public void Update_Should_ReturnFailure_When_AnswerTextsAreDuplicated()
    {
        // Arrange
        Question question = QuestionBuilder.AQuestion().Build();
        List<AnswerCandidate> invalidAnswers =
        [
            Candidate("Same", true),
            Candidate("Same", false),
            Candidate("C", false),
            Candidate("D", false),
        ];

        // Act
        Result result = question.Update(QuestionText.Create("Text?").Value, invalidAnswers);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(QuestionErrors.DuplicateAnswerText);
    }

    [Fact]
    public void Deactivate_Should_SetIsActiveToFalse_When_QuestionIsActive()
    {
        // Arrange
        Question question = QuestionBuilder.AQuestion().Build();

        // Act
        Result result = question.Deactivate();

        // Assert
        result.IsSuccess.ShouldBeTrue();
        question.IsActive.ShouldBeFalse();
    }

    [Fact]
    public void Deactivate_Should_ReturnFailure_When_QuestionIsAlreadyInactive()
    {
        // Arrange
        Question question = QuestionBuilder.AQuestion().Build();
        question.Deactivate();

        // Act
        Result result = question.Deactivate();

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(QuestionErrors.AlreadyInactive);
    }

    [Fact]
    public void Activate_Should_ReturnFailure_When_QuestionIsAlreadyActive()
    {
        // Arrange
        Question question = QuestionBuilder.AQuestion().Build();

        // Act
        Result result = question.Activate();

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(QuestionErrors.AlreadyActive);
    }

    [Fact]
    public void Activate_Should_ReturnSuccess_When_QuestionWasDeactivated()
    {
        // Arrange
        Question question = QuestionBuilder.AQuestion().Build();
        question.Deactivate();

        // Act
        Result result = question.Activate();

        // Assert
        result.IsSuccess.ShouldBeTrue();
        question.IsActive.ShouldBeTrue();
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_AnswerTextsAreDuplicated()
    {
        // Arrange
        List<AnswerCandidate> invalidAnswers =
        [
            Candidate("Same", true),
            Candidate("Same", false),
            Candidate("C", false),
            Candidate("D", false),
        ];

        // Act
        Result<Question> result = Question.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            QuestionText.Create("Text?").Value,
            invalidAnswers);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(QuestionErrors.DuplicateAnswerText);
    }

    private static AnswerCandidate Candidate(string text, bool isCorrect) =>
        new(Guid.NewGuid(), AnswerText.Create(text).Value, isCorrect);

    [Fact]
    public void NotFound_Should_IncludeQuestionIdInDescription()
    {
        // Arrange
        Guid questionId = Guid.NewGuid();

        // Act
        var error = QuestionErrors.NotFound(questionId);

        // Assert
        error.Description.ShouldContain(questionId.ToString());
    }

    [Fact]
    public void NoneAvailable_Should_IncludeCategoryIdInDescription()
    {
        // Arrange
        Guid categoryId = Guid.NewGuid();

        // Act
        var error = QuestionErrors.NoneAvailable(categoryId);

        // Assert
        error.Description.ShouldContain(categoryId.ToString());
    }
}

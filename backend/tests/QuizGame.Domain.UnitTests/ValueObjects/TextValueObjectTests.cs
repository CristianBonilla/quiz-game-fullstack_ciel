using QuizGame.Domain.SeedWork;
using QuizGame.Domain.ValueObjects;

namespace QuizGame.Domain.UnitTests.ValueObjects;

public sealed class TextValueObjectTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void QuestionTextCreate_Should_ReturnFailure_When_TextIsEmpty(string? text)
    {
        // Arrange
        // Act
        Result<QuestionText> result = QuestionText.Create(text);

        // Assert
        result.IsFailure.ShouldBeTrue();
    }

    [Fact]
    public void QuestionTextCreate_Should_ReturnFailure_When_TextExceedsMaxLength()
    {
        // Arrange
        string tooLong = new('a', QuestionText.MaxLength + 1);

        // Act
        Result<QuestionText> result = QuestionText.Create(tooLong);

        // Assert
        result.IsFailure.ShouldBeTrue();
    }

    [Fact]
    public void QuestionTextCreate_Should_TrimWhitespace_When_TextHasLeadingOrTrailingSpaces()
    {
        // Arrange
        // Act
        Result<QuestionText> result = QuestionText.Create("  What is DDD?  ");

        // Assert
        result.Value.Value.ShouldBe("What is DDD?");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void PlayerNameCreate_Should_ReturnFailure_When_NameIsEmpty(string? name)
    {
        // Arrange
        // Act
        Result<PlayerName> result = PlayerName.Create(name);

        // Assert
        result.IsFailure.ShouldBeTrue();
    }

    [Fact]
    public void PlayerNameCreate_Should_ReturnFailure_When_NameExceedsMaxLength()
    {
        // Arrange
        string tooLong = new('a', PlayerName.MaxLength + 1);

        // Act
        Result<PlayerName> result = PlayerName.Create(tooLong);

        // Assert
        result.IsFailure.ShouldBeTrue();
    }

    [Fact]
    public void PlayerNameCreate_Should_TrimWhitespace_When_NameHasLeadingOrTrailingSpaces()
    {
        // Arrange
        // Act
        Result<PlayerName> result = PlayerName.Create("  Alice  ");

        // Assert
        result.Value.Value.ShouldBe("Alice");
        result.Value.ToString().ShouldBe("Alice");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void AnswerTextCreate_Should_ReturnFailure_When_TextIsEmpty(string? text)
    {
        // Arrange
        // Act
        Result<AnswerText> result = AnswerText.Create(text);

        // Assert
        result.IsFailure.ShouldBeTrue();
    }

    [Fact]
    public void AnswerTextCreate_Should_ReturnFailure_When_TextExceedsMaxLength()
    {
        // Arrange
        string tooLong = new('a', AnswerText.MaxLength + 1);

        // Act
        Result<AnswerText> result = AnswerText.Create(tooLong);

        // Assert
        result.IsFailure.ShouldBeTrue();
    }

    [Fact]
    public void AnswerTextCreate_Should_TrimWhitespace_When_TextHasLeadingOrTrailingSpaces()
    {
        // Arrange
        // Act
        Result<AnswerText> result = AnswerText.Create("  Madrid  ");

        // Assert
        result.Value.Value.ShouldBe("Madrid");
        result.Value.ToString().ShouldBe("Madrid");
    }

    [Fact]
    public void QuestionTextCreate_Should_ReturnEqualInstances_When_ValuesMatch()
    {
        // Arrange
        QuestionText first = QuestionText.Create("Same text").Value;
        QuestionText second = QuestionText.Create("Same text").Value;

        // Act & Assert
        first.Equals(second).ShouldBeTrue();
        first.Equals(null).ShouldBeFalse();
        first.GetHashCode().ShouldBe(second.GetHashCode());
        first.ToString().ShouldBe("Same text");
    }

    [Fact]
    public void PlayerNameCreate_Should_ReturnEqualInstances_When_ValuesMatch()
    {
        // Arrange
        PlayerName first = PlayerName.Create("Alice").Value;
        PlayerName second = PlayerName.Create("Alice").Value;

        // Act & Assert
        first.Equals(second).ShouldBeTrue();
        first.Equals(null).ShouldBeFalse();
        first.GetHashCode().ShouldBe(second.GetHashCode());
    }
}

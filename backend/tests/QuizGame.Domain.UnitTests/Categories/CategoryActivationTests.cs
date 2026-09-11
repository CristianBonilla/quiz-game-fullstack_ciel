using QuizGame.Domain.Categories;
using QuizGame.Domain.SeedWork;
using QuizGame.TestUtilities;

namespace QuizGame.Domain.UnitTests.Categories;

public sealed class CategoryActivationTests
{
    [Fact]
    public void Activate_Should_ReturnFailure_When_FewerThanFiveActiveQuestionsExist()
    {
        // Arrange
        Category category = CategoryBuilder.ACategory().WithQuestions(4).Deactivated().Build();

        // Act
        Result result = category.Activate();

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(CategoryErrors.NotEnoughQuestions);
    }

    [Fact]
    public void Activate_Should_ReturnSuccess_When_AtLeastFiveActiveQuestionsExist()
    {
        // Arrange
        Category category = CategoryBuilder.ACategory().WithQuestions(5).Deactivated().Build();

        // Act
        Result result = category.Activate();

        // Assert
        result.IsSuccess.ShouldBeTrue();
        category.IsActive.ShouldBeTrue();
    }

    [Fact]
    public void Activate_Should_ReturnFailure_When_CategoryIsAlreadyActive()
    {
        // Arrange
        Category category = CategoryBuilder.ACategory().WithQuestions(5).Activated().Build();

        // Act
        Result result = category.Activate();

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(CategoryErrors.AlreadyActive);
    }

    [Fact]
    public void AddQuestion_Should_ReturnFailure_When_QuestionBelongsToAnotherCategory()
    {
        // Arrange
        Category category = CategoryBuilder.ACategory().Build();
        Domain.Questions.Question foreignQuestion = TestUtilities.QuestionBuilder.AQuestion().Build();

        // Act
        Result result = category.AddQuestion(foreignQuestion);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(CategoryErrors.QuestionFromAnotherCategory);
    }
}

using QuizGame.Domain.Categories;
using QuizGame.Domain.Questions;
using QuizGame.Domain.SeedWork;
using QuizGame.Domain.ValueObjects;
using QuizGame.TestUtilities;

namespace QuizGame.Domain.UnitTests.Categories;

public sealed class CategoryManagementTests
{
    [Fact]
    public void Update_Should_ChangeNameDescriptionAndPrize_When_InputIsValid()
    {
        // Arrange
        Category category = CategoryBuilder.ACategory().Build();
        DifficultyLevel newDifficulty = DifficultyLevel.Create(3).Value;
        Prize newPrize = Prize.Create(999m).Value;

        // Act
        Result result = category.Update("New Name", "New description", newDifficulty, newPrize);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        category.Name.ShouldBe("New Name");
        category.Description.ShouldBe("New description");
        category.DifficultyLevel.ShouldBe(newDifficulty);
        category.PrizeAmount.ShouldBe(newPrize);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Update_Should_ReturnFailure_When_NameIsEmpty(string? name)
    {
        // Arrange
        Category category = CategoryBuilder.ACategory().Build();

        // Act
        Result result = category.Update(name, "Description", DifficultyLevel.Create(1).Value, Prize.Create(100m).Value);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(CategoryErrors.EmptyName);
    }

    [Fact]
    public void Update_Should_ReturnFailure_When_NameExceedsMaxLength()
    {
        // Arrange
        Category category = CategoryBuilder.ACategory().Build();
        string tooLong = new('a', Category.NameMaxLength + 1);

        // Act
        Result result = category.Update(tooLong, "Description", DifficultyLevel.Create(1).Value, Prize.Create(100m).Value);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(CategoryErrors.NameTooLong);
    }

    [Fact]
    public void Update_Should_ReturnFailure_When_DescriptionExceedsMaxLength()
    {
        // Arrange
        Category category = CategoryBuilder.ACategory().Build();
        string tooLong = new('a', Category.DescriptionMaxLength + 1);

        // Act
        Result result = category.Update("Name", tooLong, DifficultyLevel.Create(1).Value, Prize.Create(100m).Value);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(CategoryErrors.DescriptionTooLong);
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_NameExceedsMaxLength()
    {
        // Arrange
        string tooLong = new('a', Category.NameMaxLength + 1);

        // Act
        Result<Category> result = Category.Create(
            Guid.NewGuid(),
            tooLong,
            "Description",
            DifficultyLevel.Create(1).Value,
            Prize.Create(100m).Value);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(CategoryErrors.NameTooLong);
    }

    [Fact]
    public void Create_Should_ReturnFailure_When_DescriptionExceedsMaxLength()
    {
        // Arrange
        string tooLong = new('a', Category.DescriptionMaxLength + 1);

        // Act
        Result<Category> result = Category.Create(
            Guid.NewGuid(),
            "Name",
            tooLong,
            DifficultyLevel.Create(1).Value,
            Prize.Create(100m).Value);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(CategoryErrors.DescriptionTooLong);
    }

    [Fact]
    public void AddQuestion_Should_ReturnFailure_When_QuestionIsAlreadyAdded()
    {
        // Arrange
        Category category = CategoryBuilder.ACategory().Build();
        Question question = QuestionBuilder.AQuestion().WithCategoryId(category.Id).Build();
        category.AddQuestion(question);

        // Act
        Result result = category.AddQuestion(question);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(CategoryErrors.DuplicateQuestion);
    }

    [Fact]
    public void Questions_Should_ExposeAddedQuestion()
    {
        // Arrange
        Category category = CategoryBuilder.ACategory().Build();
        Question question = QuestionBuilder.AQuestion().WithCategoryId(category.Id).Build();

        // Act
        category.AddQuestion(question);

        // Assert
        category.Questions.Count.ShouldBe(1);
        category.Questions.ShouldContain(question);
    }

    [Fact]
    public void Deactivate_Should_SetIsActiveToFalse_When_CategoryIsActive()
    {
        // Arrange
        Category category = CategoryBuilder.ACategory().WithQuestions(5).Activated().Build();

        // Act
        Result result = category.Deactivate();

        // Assert
        result.IsSuccess.ShouldBeTrue();
        category.IsActive.ShouldBeFalse();
    }

    [Fact]
    public void Deactivate_Should_ReturnFailure_When_CategoryIsAlreadyInactive()
    {
        // Arrange
        Category category = CategoryBuilder.ACategory().Build();

        // Act
        Result result = category.Deactivate();

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(CategoryErrors.AlreadyInactive);
    }

    [Fact]
    public void NotFound_Should_IncludeCategoryIdInDescription()
    {
        // Arrange
        Guid categoryId = Guid.NewGuid();

        // Act
        var error = CategoryErrors.NotFound(categoryId);

        // Assert
        error.Description.ShouldContain(categoryId.ToString());
    }

    [Fact]
    public void DuplicateName_Should_IncludeNameInDescription()
    {
        // Arrange
        // Act
        var error = CategoryErrors.DuplicateName("Software Architecture");

        // Assert
        error.Description.ShouldContain("Software Architecture");
    }
}

using QuizGame.Domain.Categories;
using QuizGame.Domain.Questions;
using QuizGame.TestUtilities;

namespace QuizGame.Domain.UnitTests.SeedWork;

/// <summary>
/// Entity&lt;TId&gt;/AggregateRoot&lt;TId&gt; equality is shared plumbing used by Game, Category and
/// Question. These tests exercise it directly through Category, a concrete AggregateRoot&lt;Guid&gt;.
/// </summary>
public sealed class EntityEqualityTests
{
    [Fact]
    public void Equals_Should_ReturnTrue_When_SameTypeAndSameId()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        Category first = CategoryBuilder.ACategory().WithId(id).Build();
        Category second = CategoryBuilder.ACategory().WithId(id).WithName("Different name").Build();

        // Act & Assert
        first.Equals(second).ShouldBeTrue();
        (first == second).ShouldBeTrue();
        (first != second).ShouldBeFalse();
        first.GetHashCode().ShouldBe(second.GetHashCode());
    }

    [Fact]
    public void Equals_Should_ReturnFalse_When_IdsDiffer()
    {
        // Arrange
        Category first = CategoryBuilder.ACategory().WithId(Guid.NewGuid()).Build();
        Category second = CategoryBuilder.ACategory().WithId(Guid.NewGuid()).Build();

        // Act & Assert
        first.Equals(second).ShouldBeFalse();
        (first != second).ShouldBeTrue();
    }

    [Fact]
    public void Equals_Should_ReturnFalse_When_ComparedToNull()
    {
        // Arrange
        Category category = CategoryBuilder.ACategory().Build();

        // Act & Assert
        category.Equals(null).ShouldBeFalse();
        (category == null).ShouldBeFalse();
        (null == category).ShouldBeFalse();
    }

    [Fact]
    public void Equals_Should_ReturnFalse_When_ComparedToUnrelatedObject()
    {
        // Arrange
        Category category = CategoryBuilder.ACategory().Build();

        // Act
        bool areEqual = category.Equals("not a category");

        // Assert
        areEqual.ShouldBeFalse();
    }

    [Fact]
    public void Equals_Should_ReturnFalse_When_SameIdButDifferentEntityType()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        Category category = CategoryBuilder.ACategory().WithId(id).Build();
        Question question = QuestionBuilder.AQuestion().WithId(id).Build();

        // Act
        bool areEqual = category.Equals(question);

        // Assert
        areEqual.ShouldBeFalse();
    }

    [Fact]
    public void TwoNullEntities_Should_BeEqual_When_ComparedWithOperator()
    {
        // Arrange
        Category? left = null;
        Category? right = null;

        // Act & Assert
        (left == right).ShouldBeTrue();
    }
}

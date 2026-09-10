using QuizGame.Domain.Categories;

namespace QuizGame.Infrastructure.Persistence.Specifications;

public sealed class CategoriesSpecification : Specification<Category>
{
    public CategoriesSpecification(bool onlyActive)
        : base(onlyActive ? category => category.IsActive : null) =>
        OrderBy = category => category.DifficultyLevel;
}

using QuizGame.Domain.Categories;

namespace QuizGame.Application.Features.Categories;

public sealed record CategoryResponse(
    Guid Id,
    string Name,
    string Description,
    int DifficultyLevel,
    decimal PrizeAmount,
    bool IsActive,
    int QuestionCount);

public static class CategoryMappings
{
    public static CategoryResponse ToResponse(this Category category)
    {
        ArgumentNullException.ThrowIfNull(category);

        return new CategoryResponse(
            category.Id,
            category.Name,
            category.Description,
            category.DifficultyLevel.Value,
            category.PrizeAmount.Amount,
            category.IsActive,
            category.Questions.Count);
    }
}

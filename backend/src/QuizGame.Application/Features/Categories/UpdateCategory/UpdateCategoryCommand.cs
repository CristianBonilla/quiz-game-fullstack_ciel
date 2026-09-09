using QuizGame.Application.Abstractions.Messaging;

namespace QuizGame.Application.Features.Categories.UpdateCategory;

public sealed record UpdateCategoryCommand(
    Guid CategoryId,
    string Name,
    string Description,
    int DifficultyLevel,
    decimal PrizeAmount) : ICommand<CategoryResponse>;

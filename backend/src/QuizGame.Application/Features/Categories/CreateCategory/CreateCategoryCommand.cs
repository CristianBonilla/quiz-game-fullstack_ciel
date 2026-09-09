using QuizGame.Application.Abstractions.Messaging;

namespace QuizGame.Application.Features.Categories.CreateCategory;

public sealed record CreateCategoryCommand(
    string Name,
    string Description,
    int DifficultyLevel,
    decimal PrizeAmount) : ICommand<CategoryResponse>;

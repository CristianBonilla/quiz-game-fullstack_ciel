using QuizGame.Application.Abstractions.Messaging;
using QuizGame.Application.Abstractions.Persistence;
using QuizGame.Domain.Categories;
using QuizGame.Domain.SeedWork;
using QuizGame.Domain.ValueObjects;

namespace QuizGame.Application.Features.Categories.CreateCategory;

public sealed class CreateCategoryCommandHandler(ICategoryRepository categories)
    : ICommandHandler<CreateCategoryCommand, CategoryResponse>
{
    public async Task<Result<CategoryResponse>> HandleAsync(
        CreateCategoryCommand command,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (await categories.NameExistsAsync(command.Name, null, cancellationToken).ConfigureAwait(false))
        {
            return Result.Failure<CategoryResponse>(CategoryErrors.DuplicateName(command.Name));
        }

        Result<DifficultyLevel> difficulty = DifficultyLevel.Create(command.DifficultyLevel);
        if (difficulty.IsFailure)
        {
            return Result.Failure<CategoryResponse>(difficulty.Error);
        }

        Result<Prize> prize = Prize.Create(command.PrizeAmount);
        if (prize.IsFailure)
        {
            return Result.Failure<CategoryResponse>(prize.Error);
        }

        Result<Category> category = Category.Create(
            Guid.NewGuid(),
            command.Name,
            command.Description,
            difficulty.Value,
            prize.Value);

        if (category.IsFailure)
        {
            return Result.Failure<CategoryResponse>(category.Error);
        }

        categories.Add(category.Value);

        return category.Value.ToResponse();
    }
}

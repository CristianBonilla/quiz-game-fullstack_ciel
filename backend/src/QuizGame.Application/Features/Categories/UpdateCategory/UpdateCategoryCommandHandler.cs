using QuizGame.Application.Abstractions.Messaging;
using QuizGame.Application.Abstractions.Persistence;
using QuizGame.Domain.Categories;
using QuizGame.Domain.SeedWork;
using QuizGame.Domain.ValueObjects;

namespace QuizGame.Application.Features.Categories.UpdateCategory;

public sealed class UpdateCategoryCommandHandler(ICategoryRepository categories)
    : ICommandHandler<UpdateCategoryCommand, CategoryResponse>
{
    public async Task<Result<CategoryResponse>> HandleAsync(
        UpdateCategoryCommand command,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        Category? category = await categories
            .GetByIdAsync(command.CategoryId, cancellationToken)
            .ConfigureAwait(false);

        if (category is null)
        {
            return Result.Failure<CategoryResponse>(CategoryErrors.NotFound(command.CategoryId));
        }

        bool duplicated = await categories
            .NameExistsAsync(command.Name, command.CategoryId, cancellationToken)
            .ConfigureAwait(false);

        if (duplicated)
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

        Result update = category.Update(command.Name, command.Description, difficulty.Value, prize.Value);

        return update.IsFailure
            ? Result.Failure<CategoryResponse>(update.Error)
            : category.ToResponse();
    }
}

using QuizGame.Application.Abstractions.Messaging;
using QuizGame.Application.Abstractions.Persistence;
using QuizGame.Domain.Categories;
using QuizGame.Domain.SeedWork;

namespace QuizGame.Application.Features.Categories.DeactivateCategory;

public sealed class DeactivateCategoryCommandHandler(ICategoryRepository categories)
    : ICommandHandler<DeactivateCategoryCommand, Unit>
{
    public async Task<Result<Unit>> HandleAsync(
        DeactivateCategoryCommand command,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        Category? category = await categories
            .GetByIdAsync(command.CategoryId, cancellationToken)
            .ConfigureAwait(false);

        if (category is null)
        {
            return Result.Failure<Unit>(CategoryErrors.NotFound(command.CategoryId));
        }

        Result deactivation = category.Deactivate();

        return deactivation.IsFailure
            ? Result.Failure<Unit>(deactivation.Error)
            : Result.Success(Unit.Value);
    }
}

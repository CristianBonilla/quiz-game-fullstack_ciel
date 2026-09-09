using QuizGame.Application.Abstractions.Messaging;
using QuizGame.Application.Abstractions.Persistence;
using QuizGame.Domain.Categories;
using QuizGame.Domain.SeedWork;

namespace QuizGame.Application.Features.Categories.GetCategoryById;

public sealed class GetCategoryByIdQueryHandler(ICategoryRepository categories)
    : IQueryHandler<GetCategoryByIdQuery, CategoryResponse>
{
    public async Task<Result<CategoryResponse>> HandleAsync(
        GetCategoryByIdQuery query,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        Category? category = await categories
            .GetByIdAsync(query.CategoryId, cancellationToken)
            .ConfigureAwait(false);

        return category is null
            ? Result.Failure<CategoryResponse>(CategoryErrors.NotFound(query.CategoryId))
            : category.ToResponse();
    }
}

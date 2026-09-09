using QuizGame.Application.Abstractions.Messaging;
using QuizGame.Application.Abstractions.Persistence;
using QuizGame.Domain.Categories;
using QuizGame.Domain.SeedWork;

namespace QuizGame.Application.Features.Categories.ListCategories;

public sealed class ListCategoriesQueryHandler(ICategoryRepository categories)
    : IQueryHandler<ListCategoriesQuery, IReadOnlyCollection<CategoryResponse>>
{
    public async Task<Result<IReadOnlyCollection<CategoryResponse>>> HandleAsync(
        ListCategoriesQuery query,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        IReadOnlyCollection<Category> found = await categories
            .ListAsync(query.OnlyActive, cancellationToken)
            .ConfigureAwait(false);

        IReadOnlyCollection<CategoryResponse> response =
            [.. found.OrderBy(category => category.DifficultyLevel.Value).Select(category => category.ToResponse())];

        return Result.Success(response);
    }
}

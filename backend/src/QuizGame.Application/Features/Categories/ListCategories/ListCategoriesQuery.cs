using QuizGame.Application.Abstractions.Messaging;

namespace QuizGame.Application.Features.Categories.ListCategories;

public sealed record ListCategoriesQuery(bool OnlyActive) : IQuery<IReadOnlyCollection<CategoryResponse>>;

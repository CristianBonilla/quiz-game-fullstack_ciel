using QuizGame.Application.Abstractions.Messaging;

namespace QuizGame.Application.Features.Categories.GetCategoryById;

public sealed record GetCategoryByIdQuery(Guid CategoryId) : IQuery<CategoryResponse>;

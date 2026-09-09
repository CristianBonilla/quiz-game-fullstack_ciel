using QuizGame.Application.Abstractions.Messaging;

namespace QuizGame.Application.Features.Categories.DeactivateCategory;

public sealed record DeactivateCategoryCommand(Guid CategoryId) : ICommand;

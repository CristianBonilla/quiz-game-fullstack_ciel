using FluentValidation;

namespace QuizGame.Application.Features.Categories.DeactivateCategory;

public sealed class DeactivateCategoryCommandValidator : AbstractValidator<DeactivateCategoryCommand>
{
    public DeactivateCategoryCommandValidator() => RuleFor(command => command.CategoryId).NotEmpty();
}

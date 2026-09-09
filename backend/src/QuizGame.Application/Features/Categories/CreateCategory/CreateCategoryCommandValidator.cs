using FluentValidation;
using QuizGame.Domain.Categories;
using QuizGame.Domain.ValueObjects;

namespace QuizGame.Application.Features.Categories.CreateCategory;

public sealed class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(command => command.Name).NotEmpty().MaximumLength(Category.NameMaxLength);
        RuleFor(command => command.Description).MaximumLength(Category.DescriptionMaxLength);
        RuleFor(command => command.DifficultyLevel)
            .InclusiveBetween(DifficultyLevel.Minimum, DifficultyLevel.Maximum);
        RuleFor(command => command.PrizeAmount).GreaterThanOrEqualTo(0m);
    }
}

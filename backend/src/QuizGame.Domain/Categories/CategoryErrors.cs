using QuizGame.Domain.SeedWork;

namespace QuizGame.Domain.Categories;

public static class CategoryErrors
{
    public static readonly Error EmptyName = Error.Validation(
        "Category.EmptyName",
        "The category name cannot be empty.");

    public static readonly Error NameTooLong = Error.Validation(
        "Category.NameTooLong",
        $"The category name cannot exceed {Category.NameMaxLength} characters.");

    public static readonly Error DescriptionTooLong = Error.Validation(
        "Category.DescriptionTooLong",
        $"The category description cannot exceed {Category.DescriptionMaxLength} characters.");

    public static readonly Error NotEnoughQuestions = Error.Conflict(
        "Category.NotEnoughQuestions",
        $"A category needs at least {Category.MinimumQuestions} questions to be activated.");

    public static readonly Error QuestionFromAnotherCategory = Error.Validation(
        "Category.QuestionFromAnotherCategory",
        "The question does not belong to this category.");

    public static readonly Error DuplicateQuestion = Error.Conflict(
        "Category.DuplicateQuestion",
        "The question is already part of this category.");

    public static readonly Error AlreadyActive = Error.Conflict(
        "Category.AlreadyActive",
        "The category is already active.");

    public static readonly Error AlreadyInactive = Error.Conflict(
        "Category.AlreadyInactive",
        "The category is already inactive.");

    public static Error NotFound(Guid categoryId) => Error.NotFound(
        "Category.NotFound",
        $"The category with identifier '{categoryId}' was not found.");

    public static Error DuplicateName(string name) => Error.Conflict(
        "Category.DuplicateName",
        $"A category named '{name}' already exists.");

    public static Error NotFoundForRound(int roundNumber) => Error.NotFound(
        "Category.NotFoundForRound",
        $"No active category is configured for round {roundNumber}.");
}

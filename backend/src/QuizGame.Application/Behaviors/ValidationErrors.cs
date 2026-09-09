using FluentValidation.Results;
using QuizGame.Domain.SeedWork;

namespace QuizGame.Application.Behaviors;

public static class ValidationErrors
{
    public static Error From(string requestName, IReadOnlyCollection<ValidationFailure> failures)
    {
        ArgumentNullException.ThrowIfNull(failures);

        string description = string.Join(
            "; ",
            failures.Select(failure => $"{failure.PropertyName}: {failure.ErrorMessage}"));

        return Error.Validation($"{requestName}.Validation", description);
    }
}

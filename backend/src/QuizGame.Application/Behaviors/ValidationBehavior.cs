using FluentValidation;
using FluentValidation.Results;
using QuizGame.Application.Abstractions.Messaging;
using QuizGame.Domain.SeedWork;

namespace QuizGame.Application.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<Result<TResponse>> HandleAsync(
        TRequest request,
        NextHandler<TResponse> continuation,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(continuation);

        IValidator<TRequest>[] applicable = [.. validators];
        if (applicable.Length == 0)
        {
            return await continuation().ConfigureAwait(false);
        }

        ValidationContext<TRequest> context = new(request);
        List<ValidationFailure> failures = [];

        foreach (IValidator<TRequest> validator in applicable)
        {
            ValidationResult validation = await validator
                .ValidateAsync(context, cancellationToken)
                .ConfigureAwait(false);

            failures.AddRange(validation.Errors.Where(failure => failure is not null));
        }

        return failures.Count > 0
            ? Result.Failure<TResponse>(ValidationErrors.From(typeof(TRequest).Name, failures))
            : await continuation().ConfigureAwait(false);
    }
}

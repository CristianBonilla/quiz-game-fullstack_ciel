using Microsoft.Extensions.Options;
using QuizGame.Application.Abstractions.Configuration;
using QuizGame.Application.Abstractions.Messaging;
using QuizGame.Domain.SeedWork;

namespace QuizGame.Application.Behaviors;

/// <summary>
/// Technical timeout only. The per-question countdown is a domain rule owned by
/// <c>GameSettings.QuestionTimeLimit</c> and must not be conflated with this one.
/// </summary>
public sealed class TimeoutBehavior<TRequest, TResponse>(IOptions<ResilienceOptions> resilienceOptions)
    : IPipelineBehavior<TRequest, TResponse>
{
    private readonly ResilienceOptions _resilience = resilienceOptions.Value;

    public async Task<Result<TResponse>> HandleAsync(
        TRequest request,
        NextHandler<TResponse> continuation,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(continuation);

        using CancellationTokenSource timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(TimeSpan.FromSeconds(_resilience.CommandTimeoutSeconds));

        try
        {
            return await continuation().ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (timeout.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
        {
            return Result.Failure<TResponse>(
                ResilienceErrors.Timeout(typeof(TRequest).Name, _resilience.CommandTimeoutSeconds));
        }
    }
}

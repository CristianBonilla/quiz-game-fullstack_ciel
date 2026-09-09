using Microsoft.Extensions.Logging;
using QuizGame.Application.Abstractions.Messaging;
using QuizGame.Domain.SeedWork;

namespace QuizGame.Application.Behaviors;

public sealed partial class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<Result<TResponse>> HandleAsync(
        TRequest request,
        NextHandler<TResponse> continuation,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(continuation);

        string requestName = typeof(TRequest).Name;
        long startedAt = TimeProvider.System.GetTimestamp();

        RequestStarted(logger, requestName);

        Result<TResponse> result = await continuation().ConfigureAwait(false);

        double elapsedMs = TimeProvider.System.GetElapsedTime(startedAt).TotalMilliseconds;

        if (result.IsSuccess)
        {
            RequestSucceeded(logger, requestName, elapsedMs);
        }
        else
        {
            RequestFailed(logger, requestName, result.Error.Code, result.Error.Type.ToString(), elapsedMs);
        }

        return result;
    }

    [LoggerMessage(EventId = 1000, Level = LogLevel.Information, Message = "Handling {RequestName}")]
    private static partial void RequestStarted(ILogger logger, string requestName);

    [LoggerMessage(
        EventId = 1001,
        Level = LogLevel.Information,
        Message = "Handled {RequestName} successfully in {ElapsedMilliseconds} ms")]
    private static partial void RequestSucceeded(ILogger logger, string requestName, double elapsedMilliseconds);

    [LoggerMessage(
        EventId = 1002,
        Level = LogLevel.Warning,
        Message = "Handled {RequestName} with error {ErrorCode} ({ErrorType}) in {ElapsedMilliseconds} ms")]
    private static partial void RequestFailed(
        ILogger logger,
        string requestName,
        string errorCode,
        string errorType,
        double elapsedMilliseconds);
}

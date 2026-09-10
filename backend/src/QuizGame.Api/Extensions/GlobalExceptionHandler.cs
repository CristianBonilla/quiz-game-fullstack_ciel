using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace QuizGame.Api.Extensions;

public sealed partial class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        UnhandledExceptionOccurred(logger, exception);

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        await httpContext.Response.WriteAsJsonAsync(
            new ProblemDetails
            {
                Title = "An unexpected error occurred.",
                Status = StatusCodes.Status500InternalServerError,
                Extensions = { ["code"] = "Server.UnhandledException" }
            },
            cancellationToken).ConfigureAwait(false);

        return true;
    }

    [LoggerMessage(EventId = 9000, Level = LogLevel.Error, Message = "Unhandled exception")]
    private static partial void UnhandledExceptionOccurred(ILogger logger, Exception exception);
}

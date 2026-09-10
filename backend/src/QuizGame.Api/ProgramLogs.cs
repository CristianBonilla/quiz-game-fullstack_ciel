using Microsoft.Extensions.Logging;

namespace QuizGame.Api;

/// <summary>Source-generated log messages for Program.cs (top-level statements can't host [LoggerMessage] members).</summary>
internal static partial class ProgramLogs
{
    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Migration attempt {Attempt}/{MaxAttempts} failed; retrying in {DelaySeconds}s.")]
    public static partial void MigrationAttemptFailed(
        ILogger logger,
        Exception exception,
        int attempt,
        int maxAttempts,
        int delaySeconds);
}

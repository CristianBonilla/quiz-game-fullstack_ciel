using QuizGame.Domain.SeedWork;

namespace QuizGame.Application.Behaviors;

public static class ResilienceErrors
{
    public static Error Timeout(string requestName, int timeoutSeconds) => Error.Failure(
        "Resilience.Timeout",
        $"'{requestName}' exceeded the {timeoutSeconds}s technical timeout.");
}

using QuizGame.Domain.SeedWork;

namespace QuizGame.Application.Behaviors;

public static class IdempotencyErrors
{
    public static Error CorruptedReplay(Guid requestId) => Error.Failure(
        "Idempotency.CorruptedReplay",
        $"The stored response for request '{requestId}' could not be replayed.");

    public static Error ConcurrentConflict(Guid requestId) => Error.Conflict(
        "Idempotency.ConcurrentConflict",
        $"Request '{requestId}' is already being processed by a concurrent call.");
}

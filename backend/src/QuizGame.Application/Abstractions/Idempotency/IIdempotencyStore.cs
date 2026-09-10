namespace QuizGame.Application.Abstractions.Idempotency;

public interface IIdempotencyStore
{
    Task<IdempotentRequest?> FindAsync(Guid requestId, CancellationToken cancellationToken);

    /// <summary>Enlists the record in the ambient unit of work so it commits with the state change.</summary>
    void Enlist(IdempotentRequest request);

    bool IsDuplicateKeyViolation(Exception exception);

    Task<int> PurgeExpiredAsync(DateTime nowUtc, CancellationToken cancellationToken);
}

namespace QuizGame.Application.Abstractions.Idempotency;

/// <summary>
/// Carries the in-flight idempotency key from <c>IdempotencyBehavior</c> down to
/// <c>UnitOfWorkBehavior</c>, so the record is enlisted in the same transaction as the state change.
/// </summary>
public sealed class IdempotencyContext
{
    public Guid? RequestId { get; private set; }

    public string? CommandName { get; private set; }

    public bool HasPendingRequest => RequestId.HasValue;

    public void Begin(Guid requestId, string commandName)
    {
        RequestId = requestId;
        CommandName = commandName;
    }

    public void Clear()
    {
        RequestId = null;
        CommandName = null;
    }
}

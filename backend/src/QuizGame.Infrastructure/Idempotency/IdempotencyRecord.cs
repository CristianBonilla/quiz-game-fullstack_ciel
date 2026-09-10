namespace QuizGame.Infrastructure.Idempotency;

public sealed class IdempotencyRecord
{
    private IdempotencyRecord(
        Guid requestId,
        string commandName,
        Guid? resourceId,
        string? responsePayload,
        int statusCode,
        DateTime createdOnUtc,
        DateTime expiresOnUtc)
    {
        RequestId = requestId;
        CommandName = commandName;
        ResourceId = resourceId;
        ResponsePayload = responsePayload;
        StatusCode = statusCode;
        CreatedOnUtc = createdOnUtc;
        ExpiresOnUtc = expiresOnUtc;
    }

    private IdempotencyRecord() => CommandName = string.Empty;

    public Guid RequestId { get; private set; }

    public string CommandName { get; private set; }

    public Guid? ResourceId { get; private set; }

    public string? ResponsePayload { get; private set; }

    public int StatusCode { get; private set; }

    public DateTime CreatedOnUtc { get; private set; }

    public DateTime ExpiresOnUtc { get; private set; }

    public static IdempotencyRecord Create(
        Guid requestId,
        string commandName,
        Guid? resourceId,
        string? responsePayload,
        int statusCode,
        DateTime createdOnUtc,
        DateTime expiresOnUtc) =>
        new(requestId, commandName, resourceId, responsePayload, statusCode, createdOnUtc, expiresOnUtc);
}

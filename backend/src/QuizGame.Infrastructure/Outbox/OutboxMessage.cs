namespace QuizGame.Infrastructure.Outbox;

public sealed class OutboxMessage
{
    private OutboxMessage(Guid id, string type, string content, DateTime occurredOnUtc)
    {
        Id = id;
        Type = type;
        Content = content;
        OccurredOnUtc = occurredOnUtc;
    }

    private OutboxMessage()
    {
        Type = string.Empty;
        Content = string.Empty;
    }

    public Guid Id { get; private set; }

    public string Type { get; private set; }

    public string Content { get; private set; }

    public DateTime OccurredOnUtc { get; private set; }

    public DateTime? ProcessedOnUtc { get; private set; }

    public int AttemptCount { get; private set; }

    public string? Error { get; private set; }

    public static OutboxMessage Create(Guid id, string type, string content, DateTime occurredOnUtc) =>
        new(id, type, content, occurredOnUtc);

    public void MarkProcessed(DateTime processedOnUtc)
    {
        ProcessedOnUtc = processedOnUtc;
        Error = null;
    }

    /// <summary>Exhausted messages stay unprocessed with an error: a logical dead-letter queue.</summary>
    public void MarkFailed(string error)
    {
        AttemptCount++;
        Error = error;
    }
}

namespace QuizGame.Application.Abstractions.Idempotency;

public sealed record IdempotentRequest(
    Guid RequestId,
    string CommandName,
    Guid? ResourceId,
    string? ResponsePayload,
    int StatusCode,
    DateTime CreatedOnUtc,
    DateTime ExpiresOnUtc);

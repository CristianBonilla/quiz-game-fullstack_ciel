using System.Text.Json;
using Microsoft.Extensions.Logging;
using QuizGame.Application.Abstractions.Idempotency;
using QuizGame.Application.Abstractions.Messaging;
using QuizGame.Application.Abstractions.Observability;
using QuizGame.Domain.SeedWork;

namespace QuizGame.Application.Behaviors;

public sealed partial class IdempotencyBehavior<TRequest, TResponse>(
    IIdempotencyStore store,
    IdempotencyContext idempotencyContext,
    ResilienceMetrics metrics,
    ILogger<IdempotencyBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
{
    internal static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    public async Task<Result<TResponse>> HandleAsync(
        TRequest request,
        NextHandler<TResponse> continuation,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(continuation);

        if (request is not IIdempotentCommand idempotent)
        {
            return await continuation().ConfigureAwait(false);
        }

        string commandName = typeof(TRequest).Name;

        Result<TResponse>? replay = await TryReplayAsync(idempotent.RequestId, commandName, cancellationToken)
            .ConfigureAwait(false);

        if (replay is not null)
        {
            return replay;
        }

        idempotencyContext.Begin(idempotent.RequestId, commandName);

        try
        {
            return await continuation().ConfigureAwait(false);
        }
        catch (Exception exception) when (store.IsDuplicateKeyViolation(exception))
        {
            // A concurrent request with the same key won the unique index; this transaction rolled
            // back untouched, so the caller gets the winner's response instead of an error.
            metrics.RecordIdempotencyConflict(commandName);
            IdempotencyConflictDetected(logger, commandName, idempotent.RequestId);

            Result<TResponse>? winner = await TryReplayAsync(idempotent.RequestId, commandName, cancellationToken)
                .ConfigureAwait(false);

            return winner ?? Result.Failure<TResponse>(IdempotencyErrors.ConcurrentConflict(idempotent.RequestId));
        }
        finally
        {
            idempotencyContext.Clear();
        }
    }

    private async Task<Result<TResponse>?> TryReplayAsync(
        Guid requestId,
        string commandName,
        CancellationToken cancellationToken)
    {
        IdempotentRequest? processed = await store.FindAsync(requestId, cancellationToken).ConfigureAwait(false);
        if (processed is null)
        {
            return null;
        }

        metrics.RecordIdempotencyReplay(commandName);
        IdempotencyReplayServed(logger, commandName, requestId);

        TResponse? payload = processed.ResponsePayload is null
            ? default
            : JsonSerializer.Deserialize<TResponse>(processed.ResponsePayload, SerializerOptions);

        return payload is null
            ? Result.Failure<TResponse>(IdempotencyErrors.CorruptedReplay(requestId))
            : Result.Success(payload);
    }

    [LoggerMessage(
        EventId = 5000,
        Level = LogLevel.Information,
        Message = "Idempotent replay served for {CommandName} with request {RequestId}")]
    private static partial void IdempotencyReplayServed(ILogger logger, string commandName, Guid requestId);

    [LoggerMessage(
        EventId = 5001,
        Level = LogLevel.Warning,
        Message = "Idempotency conflict for {CommandName} with request {RequestId}; replaying stored response")]
    private static partial void IdempotencyConflictDetected(ILogger logger, string commandName, Guid requestId);
}

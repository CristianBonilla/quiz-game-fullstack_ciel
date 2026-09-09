using System.Text.Json;
using Microsoft.Extensions.Options;
using QuizGame.Application.Abstractions;
using QuizGame.Application.Abstractions.Configuration;
using QuizGame.Application.Abstractions.Idempotency;
using QuizGame.Application.Abstractions.Messaging;
using QuizGame.Application.Abstractions.Outbox;
using QuizGame.Domain.SeedWork;

namespace QuizGame.Application.Behaviors;

public sealed class UnitOfWorkBehavior<TRequest, TResponse>(
    IUnitOfWork unitOfWork,
    IDomainEventAccumulator domainEventAccumulator,
    IOutboxWriter outboxWriter,
    IIdempotencyStore idempotencyStore,
    IdempotencyContext idempotencyContext,
    IClock clock,
    IOptions<ResilienceOptions> resilienceOptions) : IPipelineBehavior<TRequest, TResponse>
{
    private const int SuccessStatusCode = 200;

    private readonly ResilienceOptions _resilience = resilienceOptions.Value;

    public async Task<Result<TResponse>> HandleAsync(
        TRequest request,
        NextHandler<TResponse> continuation,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(continuation);

        if (request is not IBaseCommand)
        {
            return await continuation().ConfigureAwait(false);
        }

        try
        {
            Result<TResponse> result = await continuation().ConfigureAwait(false);

            if (result.IsFailure)
            {
                return result;
            }

            EnlistIdempotencyRecord(result.Value);

            IReadOnlyCollection<IDomainEvent> domainEvents = domainEventAccumulator.Collect();
            if (domainEvents.Count > 0)
            {
                await outboxWriter.WriteAsync(domainEvents, cancellationToken).ConfigureAwait(false);
            }

            // Single SaveChanges: aggregate state, outbox messages and the idempotency record
            // either all commit or none do.
            await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            return result;
        }
        finally
        {
            if (unitOfWork is IAsyncDisposable disposable)
            {
                await disposable.DisposeAsync().ConfigureAwait(false);
            }
        }
    }

    private void EnlistIdempotencyRecord(TResponse response)
    {
        if (!idempotencyContext.HasPendingRequest)
        {
            return;
        }

        DateTime now = clock.UtcNow;

        idempotencyStore.Enlist(new IdempotentRequest(
            idempotencyContext.RequestId!.Value,
            idempotencyContext.CommandName!,
            ExtractResourceId(response),
            JsonSerializer.Serialize(response, IdempotencyBehavior<TRequest, TResponse>.SerializerOptions),
            SuccessStatusCode,
            now,
            now.AddHours(_resilience.IdempotencyRetentionHours)));
    }

    private static Guid? ExtractResourceId(TResponse response) =>
        response?.GetType().GetProperty("GameId")?.GetValue(response) as Guid?;
}

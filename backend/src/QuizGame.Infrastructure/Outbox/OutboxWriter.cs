using System.Text.Json;
using QuizGame.Application.Abstractions.Outbox;
using QuizGame.Domain.SeedWork;
using QuizGame.Infrastructure.Persistence;

namespace QuizGame.Infrastructure.Outbox;

public sealed class OutboxWriter(IDbContextProvider contextProvider) : IOutboxWriter
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    public Task WriteAsync(IReadOnlyCollection<IDomainEvent> domainEvents, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(domainEvents);

        QuizGameDbContext context = contextProvider.GetContext();

        foreach (IDomainEvent domainEvent in domainEvents)
        {
            Type eventType = domainEvent.GetType();

            OutboxMessage message = OutboxMessage.Create(
                Guid.NewGuid(),
                eventType.AssemblyQualifiedName ?? eventType.FullName ?? eventType.Name,
                JsonSerializer.Serialize(domainEvent, eventType, SerializerOptions),
                domainEvent.OccurredOnUtc);

            context.OutboxMessages.Add(message);
        }

        return Task.CompletedTask;
    }
}

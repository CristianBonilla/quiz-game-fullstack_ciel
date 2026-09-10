using QuizGame.Domain.SeedWork;

namespace QuizGame.Application.Abstractions.Outbox;

public interface IOutboxWriter
{
    Task WriteAsync(IReadOnlyCollection<IDomainEvent> domainEvents, CancellationToken cancellationToken);
}

using QuizGame.Domain.SeedWork;

namespace QuizGame.Application.Abstractions.Outbox;

public interface IDomainEventAccumulator
{
    IReadOnlyCollection<IDomainEvent> Collect();
}

using Microsoft.EntityFrameworkCore.ChangeTracking;
using QuizGame.Application.Abstractions.Outbox;
using QuizGame.Domain.SeedWork;

namespace QuizGame.Infrastructure.Persistence;

public sealed class DomainEventAccumulator(IDbContextProvider contextProvider) : IDomainEventAccumulator
{
    public IReadOnlyCollection<IDomainEvent> Collect()
    {
        QuizGameDbContext? context = contextProvider.CurrentContext;
        if (context is null)
        {
            return [];
        }

        List<IDomainEvent> events = [];

        foreach (EntityEntry<IHasDomainEvents> entry in context.ChangeTracker.Entries<IHasDomainEvents>())
        {
            events.AddRange(entry.Entity.DomainEvents);
            entry.Entity.ClearDomainEvents();
        }

        return events;
    }
}

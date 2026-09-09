namespace QuizGame.Domain.SeedWork;

#pragma warning disable CA1040 // Marker interface: only aggregate roots may have a repository.
public interface IRepository<TAggregate>
    where TAggregate : class
{
}
#pragma warning restore CA1040

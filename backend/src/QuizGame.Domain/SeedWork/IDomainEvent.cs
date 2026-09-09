namespace QuizGame.Domain.SeedWork;

public interface IDomainEvent
{
    DateTime OccurredOnUtc { get; }
}

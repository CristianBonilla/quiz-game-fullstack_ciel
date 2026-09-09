using QuizGame.Domain.SeedWork;

namespace QuizGame.Domain.Games.Events;

public sealed record RoundStartedDomainEvent(
    Guid GameId,
    int RoundNumber,
    Guid QuestionId,
    decimal PrizeAtStake,
    DateTime DeadlineUtc,
    DateTime OccurredOnUtc) : IDomainEvent;

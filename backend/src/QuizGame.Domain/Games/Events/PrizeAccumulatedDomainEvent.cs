using QuizGame.Domain.SeedWork;

namespace QuizGame.Domain.Games.Events;

public sealed record PrizeAccumulatedDomainEvent(
    Guid GameId,
    int RoundNumber,
    decimal PrizeWon,
    decimal AccumulatedPrize,
    DateTime OccurredOnUtc) : IDomainEvent;

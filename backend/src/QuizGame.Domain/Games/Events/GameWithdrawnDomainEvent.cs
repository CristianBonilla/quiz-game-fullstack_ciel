using QuizGame.Domain.SeedWork;

namespace QuizGame.Domain.Games.Events;

public sealed record GameWithdrawnDomainEvent(
    Guid GameId,
    string PlayerName,
    int RoundNumber,
    decimal AccumulatedPrize,
    DateTime OccurredOnUtc) : IDomainEvent;

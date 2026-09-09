using QuizGame.Domain.SeedWork;

namespace QuizGame.Domain.Games.Events;

public sealed record RoundAdvancedDomainEvent(
    Guid GameId,
    int PreviousRoundNumber,
    int CurrentRoundNumber,
    decimal AccumulatedPrize,
    DateTime OccurredOnUtc) : IDomainEvent;

using QuizGame.Domain.SeedWork;

namespace QuizGame.Domain.Games.Events;

public sealed record GameForciblyEndedDomainEvent(
    Guid GameId,
    string PlayerName,
    int RoundNumber,
    string Reason,
    DateTime OccurredOnUtc) : IDomainEvent;

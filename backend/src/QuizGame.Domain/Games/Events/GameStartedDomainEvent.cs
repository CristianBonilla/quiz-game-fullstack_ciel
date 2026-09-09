using QuizGame.Domain.SeedWork;

namespace QuizGame.Domain.Games.Events;

public sealed record GameStartedDomainEvent(
    Guid GameId,
    string PlayerName,
    int TotalRounds,
    DateTime OccurredOnUtc) : IDomainEvent;

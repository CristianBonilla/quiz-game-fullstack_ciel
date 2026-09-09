using QuizGame.Domain.SeedWork;

namespace QuizGame.Domain.Games.Events;

public sealed record GameWonDomainEvent(
    Guid GameId,
    string PlayerName,
    decimal FinalPrize,
    DateTime OccurredOnUtc) : IDomainEvent;

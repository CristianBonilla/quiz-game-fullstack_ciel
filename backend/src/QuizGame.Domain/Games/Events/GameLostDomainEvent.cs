using QuizGame.Domain.SeedWork;

namespace QuizGame.Domain.Games.Events;

public sealed record GameLostDomainEvent(
    Guid GameId,
    string PlayerName,
    int RoundNumber,
    decimal ForfeitedPrize,
    DateTime OccurredOnUtc) : IDomainEvent;

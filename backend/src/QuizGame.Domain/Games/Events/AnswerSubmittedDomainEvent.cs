using QuizGame.Domain.SeedWork;

namespace QuizGame.Domain.Games.Events;

public sealed record AnswerSubmittedDomainEvent(
    Guid GameId,
    int RoundNumber,
    Guid QuestionId,
    Guid SelectedAnswerId,
    bool IsCorrect,
    DateTime OccurredOnUtc) : IDomainEvent;

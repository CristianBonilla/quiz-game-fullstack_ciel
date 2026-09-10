namespace QuizGame.Application.Abstractions.Notifications;

public sealed record GameStartedNotification(
    Guid GameId,
    string PlayerName,
    int TotalRounds,
    DateTime StartedOnUtc);

public sealed record RoundStartedNotification(
    Guid GameId,
    int RoundNumber,
    Guid QuestionId,
    string QuestionText,
    IReadOnlyCollection<NotificationAnswer> Answers,
    decimal PrizeAtStake,
    DateTime DeadlineUtc);

public sealed record NotificationAnswer(Guid Id, string Text);

public sealed record AnswerEvaluatedNotification(
    Guid GameId,
    int RoundNumber,
    bool IsCorrect,
    Guid CorrectAnswerId,
    decimal AccumulatedPrize,
    string Status);

public sealed record RoundAdvancedNotification(
    Guid GameId,
    int PreviousRoundNumber,
    int CurrentRoundNumber,
    decimal AccumulatedPrize);

public sealed record PrizeAccumulatedNotification(
    Guid GameId,
    int RoundNumber,
    decimal PrizeWon,
    decimal AccumulatedPrize);

public sealed record GameEndedNotification(
    Guid GameId,
    string PlayerName,
    string Status,
    decimal FinalPrize,
    DateTime EndedOnUtc);

public sealed record TimeRemainingNotification(
    Guid GameId,
    int RoundNumber,
    int SecondsRemaining);

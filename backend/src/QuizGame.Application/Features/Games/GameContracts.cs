using QuizGame.Application.Features.Questions;

namespace QuizGame.Application.Features.Games;

public sealed record GameStateResponse(
    Guid GameId,
    string PlayerName,
    string Status,
    int CurrentRound,
    int TotalRounds,
    decimal AccumulatedPrize,
    decimal PrizeAtStake,
    PlayableQuestionResponse? CurrentQuestion,
    DateTime? DeadlineUtc);

public sealed record AnswerQuestionResponse(
    Guid GameId,
    int AnsweredRound,
    bool IsCorrect,
    Guid CorrectAnswerId,
    string Status,
    decimal AccumulatedPrize,
    int CurrentRound,
    decimal PrizeAtStake,
    PlayableQuestionResponse? NextQuestion,
    DateTime? DeadlineUtc);

public sealed record RoundSummaryResponse(
    int Number,
    Guid QuestionId,
    string Outcome,
    decimal PrizeAtStake,
    Guid? SelectedAnswerId,
    DateTime? AnsweredAtUtc);

public sealed record GameSummaryResponse(
    Guid GameId,
    string PlayerName,
    string Status,
    decimal FinalPrize,
    int RoundsPlayed,
    DateTime StartedAtUtc,
    DateTime? EndedAtUtc,
    IReadOnlyCollection<RoundSummaryResponse> Rounds);

public sealed record RoundConfigurationResponse(
    int RoundNumber,
    Guid CategoryId,
    string CategoryName,
    int DifficultyLevel,
    decimal PrizeAmount,
    int AvailableQuestions);

public sealed record GameConfigurationResponse(
    int TotalRounds,
    int QuestionTimeLimitSeconds,
    IReadOnlyCollection<RoundConfigurationResponse> Rounds);

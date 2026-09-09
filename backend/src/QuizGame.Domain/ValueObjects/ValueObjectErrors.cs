using QuizGame.Domain.SeedWork;

namespace QuizGame.Domain.ValueObjects;

public static class ValueObjectErrors
{
    public static readonly Error NegativePrize =
        Error.Validation("Prize.Negative", "A prize cannot be negative.");

    public static readonly Error EmptyQuestionText =
        Error.Validation("QuestionText.Empty", "The question text cannot be empty.");

    public static readonly Error EmptyAnswerText =
        Error.Validation("AnswerText.Empty", "The answer text cannot be empty.");

    public static readonly Error EmptyPlayerName =
        Error.Validation("PlayerName.Empty", "The player name cannot be empty.");

    public static Error DifficultyLevelOutOfRange(int value) => Error.Validation(
        "DifficultyLevel.OutOfRange",
        $"The difficulty level {value} must be between {DifficultyLevel.Minimum} and {DifficultyLevel.Maximum}.");

    public static Error RoundNumberOutOfRange(int value, int totalRounds) => Error.Validation(
        "RoundNumber.OutOfRange",
        $"The round number {value} must be between 1 and {totalRounds}.");

    public static Error RoundNumberIsFinal(int value) => Error.Conflict(
        "RoundNumber.IsFinal",
        $"The round number {value} is the final one and has no successor.");

    public static Error QuestionTextTooLong(int maxLength) => Error.Validation(
        "QuestionText.TooLong",
        $"The question text cannot exceed {maxLength} characters.");

    public static Error AnswerTextTooLong(int maxLength) => Error.Validation(
        "AnswerText.TooLong",
        $"The answer text cannot exceed {maxLength} characters.");

    public static Error PlayerNameTooLong(int maxLength) => Error.Validation(
        "PlayerName.TooLong",
        $"The player name cannot exceed {maxLength} characters.");
}

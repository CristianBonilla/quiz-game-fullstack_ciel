using QuizGame.Domain.ValueObjects;

namespace QuizGame.Domain.Games.Rules;

public static class GameRules
{
    public static RoundOutcome EvaluateAnswer(Guid selectedAnswerId, Guid correctAnswerId) =>
        selectedAnswerId == correctAnswerId ? RoundOutcome.Correct : RoundOutcome.Incorrect;

    public static Prize AccumulateFor(RoundOutcome outcome, Prize accumulated, Prize atStake)
    {
        ArgumentNullException.ThrowIfNull(accumulated);
        ArgumentNullException.ThrowIfNull(atStake);

        return outcome switch
        {
            RoundOutcome.Correct => accumulated.Add(atStake),
            RoundOutcome.Incorrect => Prize.Zero,
            _ => accumulated
        };
    }

    public static GameStatus ResolveStatusAfterAnswer(RoundOutcome outcome, bool isFinalRound) =>
        outcome switch
        {
            RoundOutcome.Correct when isFinalRound => GameStatus.Won,
            RoundOutcome.Correct => GameStatus.InProgress,
            RoundOutcome.Incorrect => GameStatus.Lost,
            _ => GameStatus.InProgress
        };

    public static bool HasExpired(DateTime deadlineUtc, DateTime answeredAtUtc) =>
        answeredAtUtc > deadlineUtc;
}

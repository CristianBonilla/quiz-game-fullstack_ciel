namespace QuizGame.Domain.Games.Rules;

/// <summary>
/// NotStarted → InProgress; InProgress → InProgress | Won | Lost | Withdrawn | ForcedEnd;
/// Won, Lost, Withdrawn and ForcedEnd are terminal.
/// </summary>
public static class GameStateMachine
{
    private static readonly Dictionary<GameStatus, GameStatus[]> Transitions = new()
    {
        [GameStatus.NotStarted] = [GameStatus.InProgress],
        [GameStatus.InProgress] =
        [
            GameStatus.InProgress,
            GameStatus.Won,
            GameStatus.Lost,
            GameStatus.Withdrawn,
            GameStatus.ForcedEnd
        ],
        [GameStatus.Won] = [],
        [GameStatus.Lost] = [],
        [GameStatus.Withdrawn] = [],
        [GameStatus.ForcedEnd] = []
    };

    public static bool CanTransitionTo(GameStatus from, GameStatus to) =>
        Transitions.TryGetValue(from, out GameStatus[]? allowed) && Array.IndexOf(allowed, to) >= 0;

    public static bool IsTerminal(GameStatus status) =>
        Transitions.TryGetValue(status, out GameStatus[]? allowed) && allowed.Length == 0;

    public static IReadOnlyCollection<GameStatus> AllowedTransitionsFrom(GameStatus from) =>
        Transitions.TryGetValue(from, out GameStatus[]? allowed) ? allowed : [];
}

using QuizGame.Domain.SeedWork;
using QuizGame.Domain.ValueObjects;
using static QuizGame.Domain.SeedWork.Result;

namespace QuizGame.Domain.Games;

public sealed class GameSettings : ValueObject
{
    private readonly Dictionary<int, Prize> _prizeByRound;

    private GameSettings(int totalRounds, TimeSpan questionTimeLimit, Dictionary<int, Prize> prizeByRound)
    {
        TotalRounds = totalRounds;
        QuestionTimeLimit = questionTimeLimit;
        _prizeByRound = prizeByRound;
    }

    private GameSettings() => _prizeByRound = [];

    public int TotalRounds { get; }

    public TimeSpan QuestionTimeLimit { get; }

    public IReadOnlyDictionary<int, Prize> PrizeByRound => _prizeByRound;

    public static GameSettings Default { get; } = CreateDefault();

    public static Result<GameSettings> Create(
        int totalRounds,
        TimeSpan questionTimeLimit,
        IReadOnlyDictionary<int, Prize> prizeByRound)
    {
        ArgumentNullException.ThrowIfNull(prizeByRound);

        if (totalRounds < 1)
        {
            return Failure<GameSettings>(GameSettingsErrors.InvalidTotalRounds);
        }

        if (questionTimeLimit <= TimeSpan.Zero)
        {
            return Failure<GameSettings>(GameSettingsErrors.InvalidQuestionTimeLimit);
        }

        Dictionary<int, Prize> prizes = new(prizeByRound);
        for (int round = 1; round <= totalRounds; round++)
        {
            if (!prizes.ContainsKey(round))
            {
                return Failure<GameSettings>(GameSettingsErrors.MissingPrizeForRound(round));
            }
        }

        return new GameSettings(totalRounds, questionTimeLimit, prizes);
    }

    public Result<Prize> PrizeForRound(int roundNumber) =>
        _prizeByRound.TryGetValue(roundNumber, out Prize? prize)
            ? prize
            : Failure<Prize>(GameSettingsErrors.MissingPrizeForRound(roundNumber));

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return TotalRounds;
        yield return QuestionTimeLimit;
        foreach (KeyValuePair<int, Prize> entry in _prizeByRound.OrderBy(entry => entry.Key))
        {
            yield return entry.Key;
            yield return entry.Value;
        }
    }

    private static GameSettings CreateDefault()
    {
        Dictionary<int, Prize> prizes = new()
        {
            [1] = Prize.Create(100m).Value,
            [2] = Prize.Create(200m).Value,
            [3] = Prize.Create(400m).Value,
            [4] = Prize.Create(800m).Value,
            [5] = Prize.Create(1600m).Value
        };

        return new GameSettings(5, TimeSpan.FromSeconds(30), prizes);
    }
}

public static class GameSettingsErrors
{
    public static readonly Error InvalidTotalRounds = Error.Validation(
        "GameSettings.InvalidTotalRounds",
        "The total number of rounds must be greater than zero.");

    public static readonly Error InvalidQuestionTimeLimit = Error.Validation(
        "GameSettings.InvalidQuestionTimeLimit",
        "The question time limit must be greater than zero.");

    public static Error MissingPrizeForRound(int roundNumber) => Error.Validation(
        "GameSettings.MissingPrizeForRound",
        $"No prize is configured for round {roundNumber}.");
}

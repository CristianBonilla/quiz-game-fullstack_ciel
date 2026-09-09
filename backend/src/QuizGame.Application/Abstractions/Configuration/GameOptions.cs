namespace QuizGame.Application.Abstractions.Configuration;

public sealed class GameOptions
{
    public const string SectionName = "Game";

    public int TotalRounds { get; set; } = 5;

    public int QuestionTimeLimitSeconds { get; set; } = 30;

    public IDictionary<int, decimal> PrizeByRound { get; } = new Dictionary<int, decimal>();

    public string QuestionSelectionStrategy { get; set; } = StrategyKeys.QuestionSelection.Random;

    public string PrizeCalculationStrategy { get; set; } = StrategyKeys.PrizeCalculation.Tiered;

    public IDictionary<string, string> QuestionSelectionStrategyByCategory { get; } =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    public IDictionary<string, string> PrizeCalculationStrategyByCategory { get; } =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
}

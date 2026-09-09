namespace QuizGame.Application.Abstractions.Configuration;

public static class StrategyKeys
{
    public static class QuestionSelection
    {
        public const string Random = "random";
        public const string Weighted = "weighted";
        public const string LeastUsed = "least-used";
    }

    public static class PrizeCalculation
    {
        public const string Linear = "linear";
        public const string Exponential = "exponential";
        public const string Tiered = "tiered";
    }
}

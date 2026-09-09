using QuizGame.Domain.Categories;
using QuizGame.Domain.ValueObjects;

namespace QuizGame.Application.Strategies;

public sealed class TieredPrizeStrategy : IPrizeCalculationStrategy
{
    public Prize CalculateFor(RoundNumber round, Category category)
    {
        ArgumentNullException.ThrowIfNull(round);
        ArgumentNullException.ThrowIfNull(category);

        return round.IsFinal ? category.PrizeAmount.Add(category.PrizeAmount) : category.PrizeAmount;
    }
}

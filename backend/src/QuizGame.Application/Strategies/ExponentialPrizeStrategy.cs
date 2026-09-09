using QuizGame.Domain.Categories;
using QuizGame.Domain.SeedWork;
using QuizGame.Domain.ValueObjects;

namespace QuizGame.Application.Strategies;

public sealed class ExponentialPrizeStrategy : IPrizeCalculationStrategy
{
    public Prize CalculateFor(RoundNumber round, Category category)
    {
        ArgumentNullException.ThrowIfNull(round);
        ArgumentNullException.ThrowIfNull(category);

        decimal multiplier = (decimal)Math.Pow(2, round.Value - 1);
        Result<Prize> prize = Prize.Create(category.PrizeAmount.Amount * multiplier);

        return prize.IsSuccess ? prize.Value : Prize.Zero;
    }
}

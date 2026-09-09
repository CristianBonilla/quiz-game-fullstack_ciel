using QuizGame.Domain.Categories;
using QuizGame.Domain.SeedWork;
using QuizGame.Domain.ValueObjects;

namespace QuizGame.Application.Strategies;

public sealed class LinearPrizeStrategy : IPrizeCalculationStrategy
{
    public Prize CalculateFor(RoundNumber round, Category category)
    {
        ArgumentNullException.ThrowIfNull(round);
        ArgumentNullException.ThrowIfNull(category);

        Result<Prize> prize = Prize.Create(category.PrizeAmount.Amount * round.Value);

        return prize.IsSuccess ? prize.Value : Prize.Zero;
    }
}

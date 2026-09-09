using QuizGame.Domain.Categories;
using QuizGame.Domain.ValueObjects;

namespace QuizGame.Application.Strategies;

public interface IPrizeCalculationStrategy
{
    Prize CalculateFor(RoundNumber round, Category category);
}

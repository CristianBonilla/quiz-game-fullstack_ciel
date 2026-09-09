namespace QuizGame.Application.Strategies;

public interface IStrategyResolver<out TStrategy>
    where TStrategy : class
{
    TStrategy Resolve(string key, string fallbackKey);
}

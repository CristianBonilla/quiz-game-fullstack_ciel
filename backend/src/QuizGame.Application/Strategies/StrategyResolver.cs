using Microsoft.Extensions.DependencyInjection;

namespace QuizGame.Application.Strategies;

public sealed class StrategyResolver<TStrategy>(IServiceProvider serviceProvider) : IStrategyResolver<TStrategy>
    where TStrategy : class
{
    public TStrategy Resolve(string key, string fallbackKey) =>
        serviceProvider.GetKeyedService<TStrategy>(key)
        ?? serviceProvider.GetRequiredKeyedService<TStrategy>(fallbackKey);
}

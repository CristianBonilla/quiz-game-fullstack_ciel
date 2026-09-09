using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using QuizGame.Application.Abstractions;
using QuizGame.Application.Abstractions.Configuration;
using QuizGame.Application.Abstractions.Idempotency;
using QuizGame.Application.Abstractions.Messaging;
using QuizGame.Application.Abstractions.Notifications;
using QuizGame.Application.Abstractions.Observability;
using QuizGame.Application.Behaviors;
using QuizGame.Application.Features.Games;
using QuizGame.Application.Strategies;

namespace QuizGame.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        Assembly assembly = typeof(DependencyInjection).Assembly;

        services.AddScoped<ISender, Sender>();
        RegisterHandlers(services, assembly, typeof(ICommandHandler<,>));
        RegisterHandlers(services, assembly, typeof(IQueryHandler<,>));

        services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true);

        // Registration order defines pipeline order: outermost first. Idempotency sits outside the
        // unit of work so it can catch the unique-index violation that rolls the transaction back.
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(TimeoutBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(IdempotencyBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(UnitOfWorkBehavior<,>));

        services.AddScoped<IdempotencyContext>();
        services.AddSingleton<ResilienceMetrics>();

        services.AddScoped(typeof(IStrategyResolver<>), typeof(StrategyResolver<>));

        services.AddKeyedScoped<IQuestionSelectionStrategy, RandomQuestionSelectionStrategy>(
            StrategyKeys.QuestionSelection.Random);
        services.AddKeyedScoped<IQuestionSelectionStrategy, WeightedQuestionSelectionStrategy>(
            StrategyKeys.QuestionSelection.Weighted);
        services.AddKeyedScoped<IQuestionSelectionStrategy, LeastUsedQuestionSelectionStrategy>(
            StrategyKeys.QuestionSelection.LeastUsed);

        services.AddKeyedScoped<IPrizeCalculationStrategy, LinearPrizeStrategy>(
            StrategyKeys.PrizeCalculation.Linear);
        services.AddKeyedScoped<IPrizeCalculationStrategy, ExponentialPrizeStrategy>(
            StrategyKeys.PrizeCalculation.Exponential);
        services.AddKeyedScoped<IPrizeCalculationStrategy, TieredPrizeStrategy>(
            StrategyKeys.PrizeCalculation.Tiered);

        services.AddScoped<GameSettingsFactory>();
        services.AddScoped<RoundAssignmentService>();
        services.AddScoped<IGameNotifier, NoOpGameNotifier>();

        return services;
    }

    private static void RegisterHandlers(IServiceCollection services, Assembly assembly, Type handlerInterface)
    {
        foreach (Type implementation in assembly.GetTypes().Where(type => type is { IsAbstract: false, IsInterface: false }))
        {
            IEnumerable<Type> contracts = implementation
                .GetInterfaces()
                .Where(contract => contract.IsGenericType
                    && contract.GetGenericTypeDefinition() == handlerInterface);

            foreach (Type contract in contracts)
            {
                services.AddScoped(contract, implementation);
            }
        }
    }
}

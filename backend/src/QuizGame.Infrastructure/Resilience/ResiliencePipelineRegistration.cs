using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;
using QuizGame.Application.Abstractions;
using QuizGame.Application.Abstractions.Configuration;
using QuizGame.Application.Abstractions.Observability;

namespace QuizGame.Infrastructure.Resilience;

public static partial class ResiliencePipelineRegistration
{
    public static IServiceCollection AddOutboxResiliencePipeline(this IServiceCollection services)
    {
        services.AddSingleton<CircuitBreakerState>();

        services.AddResiliencePipeline(ResiliencePipelineNames.OutboxDispatch, static (builder, context) =>
        {
            OutboxResilienceOptions options = context.ServiceProvider
                .GetRequiredService<IOptions<ResilienceOptions>>().Value.Outbox;

            ResilienceMetrics metrics = context.ServiceProvider.GetRequiredService<ResilienceMetrics>();
            CircuitBreakerState circuitState = context.ServiceProvider.GetRequiredService<CircuitBreakerState>();
            IClock clock = context.ServiceProvider.GetRequiredService<IClock>();
            ILogger logger = context.ServiceProvider
                .GetRequiredService<ILoggerFactory>()
                .CreateLogger(ResiliencePipelineNames.OutboxDispatch);

            builder
                .AddRetry(new RetryStrategyOptions
                {
                    MaxRetryAttempts = options.MaxRetryAttempts,
                    BackoffType = DelayBackoffType.Exponential,
                    UseJitter = true,
                    Delay = TimeSpan.FromSeconds(options.BaseDelaySeconds),
                    // Business failures are Result.Failure values, never exceptions, so they can
                    // never reach this predicate and can never be retried.
                    ShouldHandle = new PredicateBuilder().Handle<TransientDispatchException>(),
                    OnRetry = arguments =>
                    {
                        metrics.RecordOutboxRetry();
                        OutboxRetryScheduled(logger, arguments.AttemptNumber, arguments.RetryDelay.TotalMilliseconds);
                        return ValueTask.CompletedTask;
                    }
                })
                .AddTimeout(new TimeoutStrategyOptions
                {
                    Timeout = TimeSpan.FromSeconds(options.DispatchTimeoutSeconds)
                })
                .AddCircuitBreaker(new CircuitBreakerStrategyOptions
                {
                    FailureRatio = options.CircuitFailureRatio,
                    SamplingDuration = TimeSpan.FromSeconds(options.CircuitSamplingDurationSeconds),
                    MinimumThroughput = options.CircuitMinimumThroughput,
                    BreakDuration = TimeSpan.FromSeconds(options.CircuitBreakDurationSeconds),
                    ShouldHandle = new PredicateBuilder().Handle<TransientDispatchException>(),
                    OnOpened = arguments =>
                    {
                        circuitState.MarkOpened(clock.UtcNow);
                        metrics.RecordCircuitOpened();
                        CircuitOpened(logger, arguments.BreakDuration.TotalSeconds);
                        return ValueTask.CompletedTask;
                    },
                    OnClosed = _ =>
                    {
                        circuitState.MarkClosed();
                        CircuitClosed(logger);
                        return ValueTask.CompletedTask;
                    },
                    OnHalfOpened = _ =>
                    {
                        circuitState.MarkHalfOpened();
                        CircuitHalfOpened(logger);
                        return ValueTask.CompletedTask;
                    }
                });
        });

        return services;
    }

    [LoggerMessage(
        EventId = 6000,
        Level = LogLevel.Warning,
        Message = "Outbox dispatch retry {AttemptNumber} scheduled in {DelayMilliseconds} ms")]
    private static partial void OutboxRetryScheduled(ILogger logger, int attemptNumber, double delayMilliseconds);

    [LoggerMessage(EventId = 6001, Level = LogLevel.Error, Message = "Outbox circuit opened for {BreakSeconds} s")]
    private static partial void CircuitOpened(ILogger logger, double breakSeconds);

    [LoggerMessage(EventId = 6002, Level = LogLevel.Information, Message = "Outbox circuit closed")]
    private static partial void CircuitClosed(ILogger logger);

    [LoggerMessage(EventId = 6003, Level = LogLevel.Information, Message = "Outbox circuit half-opened")]
    private static partial void CircuitHalfOpened(ILogger logger);
}

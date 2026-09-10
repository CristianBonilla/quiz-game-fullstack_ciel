using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace QuizGame.Infrastructure.Resilience;

public sealed class OutboxCircuitHealthCheck(CircuitBreakerState state) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyDictionary<string, object> data = new Dictionary<string, object>
        {
            ["circuitState"] = state.Current.ToString(),
            ["openCount"] = state.OpenCount,
            ["lastOpenedOnUtc"] = state.LastOpenedOnUtc?.ToString("O") ?? "never"
        };

        return Task.FromResult(state.IsHealthy
            ? HealthCheckResult.Healthy("Outbox circuit is closed.", data)
            : HealthCheckResult.Degraded("Outbox circuit is open.", data: data));
    }
}

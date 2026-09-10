using Polly.CircuitBreaker;

namespace QuizGame.Infrastructure.Resilience;

/// <summary>Exposes the outbox circuit state so a health check can surface it.</summary>
public sealed class CircuitBreakerState
{
    private int _openCount;

    public CircuitState Current { get; private set; } = CircuitState.Closed;

    public DateTime? LastOpenedOnUtc { get; private set; }

    public int OpenCount => _openCount;

    public bool IsHealthy => Current is CircuitState.Closed or CircuitState.HalfOpen;

    public void MarkOpened(DateTime nowUtc)
    {
        Current = CircuitState.Open;
        LastOpenedOnUtc = nowUtc;
        Interlocked.Increment(ref _openCount);
    }

    public void MarkHalfOpened() => Current = CircuitState.HalfOpen;

    public void MarkClosed() => Current = CircuitState.Closed;
}

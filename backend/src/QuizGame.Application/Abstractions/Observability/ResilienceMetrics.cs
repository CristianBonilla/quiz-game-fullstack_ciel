using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace QuizGame.Application.Abstractions.Observability;

public sealed class ResilienceMetrics : IDisposable
{
    public const string MeterName = "QuizGame.Resilience";
    public const string ActivitySourceName = "QuizGame";

    private readonly Meter _meter;
    private readonly Counter<long> _idempotencyHits;
    private readonly Counter<long> _idempotencyConflicts;
    private readonly Counter<long> _outboxRetries;
    private readonly Counter<long> _circuitOpened;

    public ResilienceMetrics()
    {
        _meter = new Meter(MeterName);
        _idempotencyHits = _meter.CreateCounter<long>("quizgame.idempotency.replays");
        _idempotencyConflicts = _meter.CreateCounter<long>("quizgame.idempotency.conflicts");
        _outboxRetries = _meter.CreateCounter<long>("quizgame.outbox.retries");
        _circuitOpened = _meter.CreateCounter<long>("quizgame.outbox.circuit_opened");
    }

    public static ActivitySource ActivitySource { get; } = new(ActivitySourceName);

    public void RecordIdempotencyReplay(string commandName) =>
        _idempotencyHits.Add(1, new KeyValuePair<string, object?>("command", commandName));

    public void RecordIdempotencyConflict(string commandName) =>
        _idempotencyConflicts.Add(1, new KeyValuePair<string, object?>("command", commandName));

    public void RecordOutboxRetry() => _outboxRetries.Add(1);

    public void RecordCircuitOpened() => _circuitOpened.Add(1);

    public void Dispose() => _meter.Dispose();
}

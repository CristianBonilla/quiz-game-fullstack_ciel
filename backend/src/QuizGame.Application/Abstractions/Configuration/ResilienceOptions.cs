using System.ComponentModel.DataAnnotations;

namespace QuizGame.Application.Abstractions.Configuration;

public sealed class ResilienceOptions
{
    public const string SectionName = "Resilience";

    [Range(1, 600)]
    public int CommandTimeoutSeconds { get; set; } = 30;

    [Range(1, 168)]
    public int IdempotencyRetentionHours { get; set; } = 24;

    [Range(1, 1440)]
    public int IdempotencyCleanupIntervalMinutes { get; set; } = 60;

    [Required]
    public OutboxResilienceOptions Outbox { get; set; } = new();
}

public sealed class OutboxResilienceOptions
{
    [Range(1, 60)]
    public int PollingIntervalSeconds { get; set; } = 2;

    [Range(1, 500)]
    public int BatchSize { get; set; } = 20;

    [Range(1, 20)]
    public int MaxRetryAttempts { get; set; } = 5;

    [Range(1, 60)]
    public int BaseDelaySeconds { get; set; } = 1;

    [Range(1, 300)]
    public int DispatchTimeoutSeconds { get; set; } = 10;

    [Range(0.01, 1.0)]
    public double CircuitFailureRatio { get; set; } = 0.5;

    [Range(1, 600)]
    public int CircuitSamplingDurationSeconds { get; set; } = 30;

    [Range(2, 1000)]
    public int CircuitMinimumThroughput { get; set; } = 8;

    [Range(1, 600)]
    public int CircuitBreakDurationSeconds { get; set; } = 15;
}

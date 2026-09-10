using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QuizGame.Application.Abstractions;
using QuizGame.Application.Abstractions.Configuration;
using QuizGame.Application.Abstractions.Idempotency;

namespace QuizGame.Infrastructure.Idempotency;

public sealed partial class IdempotencyCleanupService(
    IIdempotencyStore store,
    IClock clock,
    IOptions<ResilienceOptions> resilienceOptions,
    ILogger<IdempotencyCleanupService> logger) : BackgroundService
{
    private readonly ResilienceOptions _resilience = resilienceOptions.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using PeriodicTimer timer = new(TimeSpan.FromMinutes(_resilience.IdempotencyCleanupIntervalMinutes));

        while (await timer.WaitForNextTickAsync(stoppingToken).ConfigureAwait(false))
        {
            try
            {
                int purged = await store.PurgeExpiredAsync(clock.UtcNow, stoppingToken).ConfigureAwait(false);
                if (purged > 0)
                {
                    ExpiredRecordsPurged(logger, purged);
                }
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                PurgeFailed(logger, exception);
            }
        }
    }

    [LoggerMessage(EventId = 7000, Level = LogLevel.Information, Message = "Purged {Count} expired idempotency records")]
    private static partial void ExpiredRecordsPurged(ILogger logger, int count);

    [LoggerMessage(EventId = 7001, Level = LogLevel.Warning, Message = "Idempotency purge failed")]
    private static partial void PurgeFailed(ILogger logger, Exception exception);
}

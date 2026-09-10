using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QuizGame.Application.Abstractions.Idempotency;
using QuizGame.Infrastructure.Persistence;

namespace QuizGame.Infrastructure.Idempotency;

public sealed class IdempotencyStore(
    IDbContextProvider contextProvider,
    IDbContextFactory<QuizGameDbContext> factory) : IIdempotencyStore
{
    private const int UniqueIndexViolation = 2601;
    private const int PrimaryKeyViolation = 2627;

    public async Task<IdempotentRequest?> FindAsync(Guid requestId, CancellationToken cancellationToken)
    {
        QuizGameDbContext context = contextProvider.GetContext();

        IdempotencyRecord? record = await context.IdempotencyRecords
            .AsNoTracking()
            .FirstOrDefaultAsync(existing => existing.RequestId == requestId, cancellationToken)
            .ConfigureAwait(false);

        return record is null
            ? null
            : new IdempotentRequest(
                record.RequestId,
                record.CommandName,
                record.ResourceId,
                record.ResponsePayload,
                record.StatusCode,
                record.CreatedOnUtc,
                record.ExpiresOnUtc);
    }

    public void Enlist(IdempotentRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        contextProvider.GetContext().IdempotencyRecords.Add(IdempotencyRecord.Create(
            request.RequestId,
            request.CommandName,
            request.ResourceId,
            request.ResponsePayload,
            request.StatusCode,
            request.CreatedOnUtc,
            request.ExpiresOnUtc));
    }

    public bool IsDuplicateKeyViolation(Exception exception) =>
        exception is DbUpdateException { InnerException: SqlException sql }
        && sql.Number is UniqueIndexViolation or PrimaryKeyViolation;

    public async Task<int> PurgeExpiredAsync(DateTime nowUtc, CancellationToken cancellationToken)
    {
        await using QuizGameDbContext context = await factory
            .CreateDbContextAsync(cancellationToken)
            .ConfigureAwait(false);

        return await context.IdempotencyRecords
            .Where(record => record.ExpiresOnUtc <= nowUtc)
            .ExecuteDeleteAsync(cancellationToken)
            .ConfigureAwait(false);
    }
}

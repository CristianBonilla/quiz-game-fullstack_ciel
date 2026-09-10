using Microsoft.EntityFrameworkCore;
using QuizGame.Domain.SeedWork;

namespace QuizGame.Infrastructure.Persistence;

public sealed class DbContextProvider(IDbContextFactory<QuizGameDbContext> factory)
    : IDbContextProvider, IUnitOfWork, IAsyncDisposable
{
    private QuizGameDbContext? _context;

    public QuizGameDbContext? CurrentContext => _context;

    // One context per command, created lazily from the factory: cheap because it opens no
    // connection until the first query, and safe under SignalR's connection-scoped DI.
    public QuizGameDbContext GetContext() => _context ??= factory.CreateDbContext();

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context is null ? 0 : await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

    public async ValueTask DisposeAsync()
    {
        if (_context is not null)
        {
            await _context.DisposeAsync().ConfigureAwait(false);
            _context = null;
        }
    }
}

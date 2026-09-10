using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuizGame.Infrastructure.Persistence;
using Testcontainers.MsSql;

namespace QuizGame.Api.IntegrationTests.Integration;

/// <summary>
/// Boots the real API in-process against a disposable SQL Server container and applies migrations,
/// so tests exercise the same wiring as production (real Game/Question/Category entities, real EF Core,
/// only the container is test infrastructure). Gracefully degrades to "skip" when Docker is unavailable
/// (e.g. CI runners without a Docker daemon) instead of failing the whole suite.
/// </summary>
public sealed class QuizGameApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private MsSqlContainer? _container;

    public bool IsDockerAvailable { get; private set; }

    public async Task InitializeAsync()
    {
        try
        {
            _container = new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest").Build();

            await _container.StartAsync().ConfigureAwait(false);
            IsDockerAvailable = true;
        }
        catch (Exception)
        {
            // No Docker daemon reachable (common on developer machines/CI without container support).
            // Tests using this factory check IsDockerAvailable and skip themselves cleanly.
            IsDockerAvailable = false;
            return;
        }

        using IServiceScope scope = Services.CreateScope();
        IDbContextFactory<QuizGameDbContext> dbContextFactory =
            scope.ServiceProvider.GetRequiredService<IDbContextFactory<QuizGameDbContext>>();
        await using QuizGameDbContext dbContext = await dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);
        await dbContext.Database.MigrateAsync().ConfigureAwait(false);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            if (_container is null)
            {
                return;
            }

            configuration.AddInMemoryCollection(
            [
                new("ConnectionStrings:DefaultConnection", _container.GetConnectionString()),
            ]);
        });
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        if (_container is not null)
        {
            await _container.DisposeAsync().ConfigureAwait(false);
        }

        await base.DisposeAsync().ConfigureAwait(false);
    }
}

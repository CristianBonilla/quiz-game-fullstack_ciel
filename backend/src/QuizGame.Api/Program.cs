using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using QuizGame.Api;
using QuizGame.Api.Extensions;
using QuizGame.Api.Hubs;
using QuizGame.Application;
using QuizGame.Infrastructure;
using QuizGame.Infrastructure.Persistence;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

string connectionString = ConnectionStringResolver.Resolve(builder.Configuration);

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration, connectionString)
    .AddApiServices(builder.Configuration);

WebApplication app = builder.Build();

app.UseExceptionHandler();
app.UseCors(CorsPolicies.Frontend);

app.MapOpenApi();
app.MapScalarApiReference();

app.MapEndpointModules();
app.MapHub<GameHub>("/hubs/game");

app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = check => check.Tags.Contains("live") });
app.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = check => check.Tags.Contains("ready") });

// Migrating on startup is convenient for the Docker Compose demo only. In a real deployment this
// would be a separate release step/job, never something that races the app's own startup.
if (app.Environment.IsEnvironment("Docker"))
{
    await MigrateDatabaseWithRetryAsync(app.Services, app.Logger).ConfigureAwait(false);
}

await app.RunAsync().ConfigureAwait(false);

static async Task MigrateDatabaseWithRetryAsync(IServiceProvider services, ILogger logger)
{
    const int maxAttempts = 10;
    using IServiceScope scope = services.CreateScope();
    IDbContextFactory<QuizGameDbContext> dbContextFactory =
        scope.ServiceProvider.GetRequiredService<IDbContextFactory<QuizGameDbContext>>();

    for (int attempt = 1; attempt <= maxAttempts; attempt++)
    {
        try
        {
            await using QuizGameDbContext dbContext = await dbContextFactory.CreateDbContextAsync().ConfigureAwait(false);
            await dbContext.Database.MigrateAsync().ConfigureAwait(false);
            return;
        }
        catch (Exception exception) when (attempt < maxAttempts)
        {
            ProgramLogs.MigrationAttemptFailed(logger, exception, attempt, maxAttempts, 3 * attempt);
            await Task.Delay(TimeSpan.FromSeconds(3 * attempt)).ConfigureAwait(false);
        }
    }
}

/// <summary>Exposed so WebApplicationFactory&lt;Program&gt; can bootstrap the API in-process for integration tests.</summary>
public partial class Program;

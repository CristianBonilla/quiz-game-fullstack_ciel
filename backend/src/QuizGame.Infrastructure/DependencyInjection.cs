using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuizGame.Application.Abstractions;
using QuizGame.Application.Abstractions.Configuration;
using QuizGame.Application.Abstractions.Idempotency;
using QuizGame.Application.Abstractions.Outbox;
using QuizGame.Application.Abstractions.Persistence;
using QuizGame.Domain.SeedWork;
using QuizGame.Infrastructure.Idempotency;
using QuizGame.Infrastructure.Outbox;
using QuizGame.Infrastructure.Persistence;
using QuizGame.Infrastructure.Persistence.Repositories;
using QuizGame.Infrastructure.Resilience;
using QuizGame.Infrastructure.Support;

namespace QuizGame.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        string connectionString)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        services.AddOptions<ResilienceOptions>()
            .Bind(configuration.GetSection(ResilienceOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        int commandTimeout = configuration
            .GetSection(ResilienceOptions.SectionName)
            .GetValue("CommandTimeoutSeconds", 30);

        // A DbContext factory (not a scoped DbContext) is required because SignalR hub scopes span
        // the whole connection; a scoped context there would leak tracked entities across messages.
        services.AddDbContextFactory<QuizGameDbContext>(options =>
            options.UseSqlServer(connectionString, sql =>
            {
                // EF's execution strategy is the ONLY retry layer against SQL Server. Wrapping it
                // with Polly would multiply attempts and break explicit transactions under retry.
                sql.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(10), errorNumbersToAdd: null);
                sql.CommandTimeout(commandTimeout);
                sql.MigrationsHistoryTable("__EFMigrationsHistory", "dbo");
            }));

        services.AddScoped<DbContextProvider>();
        services.AddScoped<IDbContextProvider>(provider => provider.GetRequiredService<DbContextProvider>());
        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<DbContextProvider>());
        services.AddScoped<IDomainEventAccumulator, DomainEventAccumulator>();

        services.AddScoped<IGameRepository, GameRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IQuestionRepository, QuestionRepository>();

        services.AddScoped<IIdempotencyStore, IdempotencyStore>();
        services.AddScoped<IOutboxWriter, OutboxWriter>();

        services.AddSingleton<IClock, SystemClock>();
        services.AddSingleton<IRandomProvider, RandomProvider>();

        services.AddOutboxResiliencePipeline();

        services.AddHostedService<OutboxProcessor>();
        services.AddHostedService<IdempotencyCleanupService>();

        return services;
    }
}

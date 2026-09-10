using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace QuizGame.Infrastructure.Persistence;

public sealed class QuizGameDbContextFactory : IDesignTimeDbContextFactory<QuizGameDbContext>
{
    public QuizGameDbContext CreateDbContext(string[] args)
    {
        // dotnet-ef sets ASPNETCORE_ENVIRONMENT=Development itself when the variable isn't already
        // present in the caller's shell; treat that tool default the same as "unset" and fall back
        // to the Local profile, so only an explicit ASPNETCORE_ENVIRONMENT=Docker set by the caller
        // switches the design-time factory to the Docker profile.
        string? environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (string.IsNullOrEmpty(environmentName) || string.Equals(environmentName, "Development", StringComparison.OrdinalIgnoreCase))
        {
            environmentName = "Local";
        }

        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "..", "QuizGame.Api"))
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        string connectionString = ConnectionStringResolver.Resolve(configuration);

        DbContextOptionsBuilder<QuizGameDbContext> optionsBuilder = new();
        optionsBuilder.UseSqlServer(connectionString, sql =>
            sql.MigrationsHistoryTable("__EFMigrationsHistory", "dbo"));

        return new QuizGameDbContext(optionsBuilder.Options);
    }
}

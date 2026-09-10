using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace QuizGame.Infrastructure.Persistence;

/// <summary>
/// Resolves the "DefaultConnection" string for the active profile, substituting the
/// <c>${MSSQL_SA_PASSWORD}</c> placeholder (Docker profile only) with the value of the
/// same-named environment variable.
/// </summary>
/// <remarks>
/// Prompt 09 asks for this type to live in <c>QuizGame.Api/Extensions</c>. It stays here instead
/// because <see cref="QuizGameDbContextFactory"/> (the EF Core design-time factory, per Prompt 05)
/// must reuse the exact same logic, and Infrastructure cannot depend on the Api project. Keeping a
/// single implementation in the lowest layer both callers can reach beats matching the suggested
/// path literally.
/// </remarks>
public static class ConnectionStringResolver
{
    private const string PasswordPlaceholder = "${MSSQL_SA_PASSWORD}";
    private const string PasswordEnvironmentVariable = "MSSQL_SA_PASSWORD";

    public static string Resolve(IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        string connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

        if (!connectionString.Contains(PasswordPlaceholder, StringComparison.Ordinal))
        {
            return connectionString;
        }

        string password = configuration[PasswordEnvironmentVariable]
            ?? throw new InvalidOperationException(
                $"Environment variable '{PasswordEnvironmentVariable}' is required by the current profile.");

        // SqlConnectionStringBuilder escapes the value: a textual Replace would corrupt the
        // connection string (or worse, inject into it) if the password contains ';', '\'' or '"'.
        SqlConnectionStringBuilder builder = new(connectionString.Replace(PasswordPlaceholder, string.Empty, StringComparison.Ordinal))
        {
            Password = password
        };

        return builder.ConnectionString;
    }
}


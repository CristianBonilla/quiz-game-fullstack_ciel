using System.Reflection;

namespace QuizGame.Infrastructure.Persistence.StoredProcedures;

public static class EmbeddedSqlScripts
{
    public static string Read(string fileName)
    {
        Assembly assembly = typeof(EmbeddedSqlScripts).Assembly;
        string resourceName = $"{assembly.GetName().Name}.Persistence.StoredProcedures.{fileName}";

        using Stream stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new FileNotFoundException($"Embedded SQL script '{resourceName}' was not found.");
        using StreamReader reader = new(stream);

        return reader.ReadToEnd();
    }
}

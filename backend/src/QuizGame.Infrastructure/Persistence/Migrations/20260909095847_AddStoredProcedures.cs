using Microsoft.EntityFrameworkCore.Migrations;
using QuizGame.Infrastructure.Persistence.StoredProcedures;

#nullable disable

namespace QuizGame.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddStoredProcedures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(AsDynamicSql(EmbeddedSqlScripts.Read("sp_GetRandomQuestionByCategory_v1.sql")));
            migrationBuilder.Sql(AsDynamicSql(EmbeddedSqlScripts.Read("sp_GetGameSummary_v1.sql")));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS dbo.sp_GetRandomQuestionByCategory_v1;");
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS dbo.sp_GetGameSummary_v1;");
        }

        // CREATE PROCEDURE must be the only statement in its batch; wrapping it in dynamic SQL
        // lets it live inside the IF NOT EXISTS guard that --idempotent scripts generate.
        private static string AsDynamicSql(string script) =>
            $"EXEC(N'{script.Replace("'", "''", StringComparison.Ordinal)}');";
    }
}

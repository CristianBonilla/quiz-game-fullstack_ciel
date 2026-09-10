BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [dbo].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909095847_AddStoredProcedures'
)
BEGIN
    EXEC(N'CREATE OR ALTER PROCEDURE dbo.sp_GetRandomQuestionByCategory_v1
        @CategoryId UNIQUEIDENTIFIER,
        @ExcludedQuestionIds NVARCHAR(MAX) = NULL
    AS
    BEGIN
        SET NOCOUNT ON;

        SELECT TOP (1) q.Id, q.CategoryId, q.Text, q.IsActive
        FROM dbo.Questions AS q
        WHERE q.CategoryId = @CategoryId
          AND q.IsActive = 1
          AND (
              @ExcludedQuestionIds IS NULL
              OR q.Id NOT IN (SELECT CAST(value AS UNIQUEIDENTIFIER) FROM STRING_SPLIT(@ExcludedQuestionIds, '',''))
          )
        ORDER BY NEWID();
    END
    ');
END;

IF NOT EXISTS (
    SELECT * FROM [dbo].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909095847_AddStoredProcedures'
)
BEGIN
    EXEC(N'CREATE OR ALTER PROCEDURE dbo.sp_GetGameSummary_v1
        @GameId UNIQUEIDENTIFIER
    AS
    BEGIN
        SET NOCOUNT ON;

        SELECT g.Id, g.PlayerName, g.Status, g.AccumulatedPrize, g.StartedAtUtc, g.EndedAtUtc
        FROM dbo.Games AS g
        WHERE g.Id = @GameId;

        SELECT r.Number, r.QuestionId, r.Outcome, r.PrizeAtStake, r.SelectedAnswerId, r.AnsweredAtUtc
        FROM dbo.Rounds AS r
        WHERE r.GameId = @GameId
        ORDER BY r.Number;
    END
    ');
END;

IF NOT EXISTS (
    SELECT * FROM [dbo].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909095847_AddStoredProcedures'
)
BEGIN
    INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260909095847_AddStoredProcedures', N'10.0.0');
END;

COMMIT;
GO


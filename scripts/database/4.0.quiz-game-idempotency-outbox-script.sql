BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [dbo].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909095933_AddIdempotencyAndOutbox'
)
BEGIN
    INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260909095933_AddIdempotencyAndOutbox', N'10.0.0');
END;

COMMIT;
GO


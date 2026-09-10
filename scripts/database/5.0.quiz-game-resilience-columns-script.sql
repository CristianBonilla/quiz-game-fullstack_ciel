BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [dbo].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909103808_AddResilienceColumns'
)
BEGIN
    EXEC sp_rename N'[dbo].[IdempotencyRecords].[SerializedResponse]', N'ResponsePayload', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [dbo].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909103808_AddResilienceColumns'
)
BEGIN
    EXEC sp_rename N'[dbo].[IdempotencyRecords].[RequestName]', N'CommandName', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [dbo].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909103808_AddResilienceColumns'
)
BEGIN
    ALTER TABLE [dbo].[OutboxMessages] ADD [AttemptCount] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [dbo].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909103808_AddResilienceColumns'
)
BEGIN
    ALTER TABLE [dbo].[IdempotencyRecords] ADD [ExpiresOnUtc] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [dbo].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909103808_AddResilienceColumns'
)
BEGIN
    ALTER TABLE [dbo].[IdempotencyRecords] ADD [ResourceId] uniqueidentifier NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [dbo].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909103808_AddResilienceColumns'
)
BEGIN
    ALTER TABLE [dbo].[IdempotencyRecords] ADD [StatusCode] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [dbo].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909103808_AddResilienceColumns'
)
BEGIN
    CREATE INDEX [IX_IdempotencyRecords_ExpiresOnUtc] ON [dbo].[IdempotencyRecords] ([ExpiresOnUtc]);
END;

IF NOT EXISTS (
    SELECT * FROM [dbo].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909103808_AddResilienceColumns'
)
BEGIN
    CREATE UNIQUE INDEX [UQ_IdempotencyRecords_RequestId] ON [dbo].[IdempotencyRecords] ([RequestId]);
END;

IF NOT EXISTS (
    SELECT * FROM [dbo].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909103808_AddResilienceColumns'
)
BEGIN
    INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260909103808_AddResilienceColumns', N'10.0.0');
END;

COMMIT;
GO


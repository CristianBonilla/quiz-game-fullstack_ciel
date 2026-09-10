IF OBJECT_ID(N'[dbo].[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [dbo].[__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [dbo].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909093920_InitialCreate'
)
BEGIN
    CREATE TABLE [dbo].[Categories] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [Description] nvarchar(400) NOT NULL,
        [DifficultyLevel] int NOT NULL,
        [PrizeAmount] decimal(18,2) NOT NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_Categories] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [dbo].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909093920_InitialCreate'
)
BEGIN
    CREATE TABLE [dbo].[Games] (
        [Id] uniqueidentifier NOT NULL,
        [PlayerName] nvarchar(60) NOT NULL,
        [TotalRounds] int NOT NULL,
        [QuestionTimeLimit] time NOT NULL,
        [Status] nvarchar(20) NOT NULL,
        [CurrentRound] int NOT NULL,
        [AccumulatedPrize] decimal(18,2) NOT NULL,
        [StartedAtUtc] datetime2 NOT NULL,
        [EndedAtUtc] datetime2 NULL,
        [RowVersion] rowversion NULL,
        CONSTRAINT [PK_Games] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [dbo].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909093920_InitialCreate'
)
BEGIN
    CREATE TABLE [dbo].[Questions] (
        [Id] uniqueidentifier NOT NULL,
        [CategoryId] uniqueidentifier NOT NULL,
        [Text] nvarchar(500) NOT NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_Questions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Questions_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Categories] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [dbo].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909093920_InitialCreate'
)
BEGIN
    CREATE TABLE [dbo].[Rounds] (
        [Id] uniqueidentifier NOT NULL,
        [GameId] uniqueidentifier NOT NULL,
        [Number] int NOT NULL,
        [QuestionId] uniqueidentifier NOT NULL,
        [CorrectAnswerId] uniqueidentifier NOT NULL,
        [PrizeAtStake] decimal(18,2) NOT NULL,
        [SelectedAnswerId] uniqueidentifier NULL,
        [Outcome] nvarchar(20) NOT NULL,
        [DeadlineUtc] datetime2 NOT NULL,
        [AnsweredAtUtc] datetime2 NULL,
        CONSTRAINT [PK_Rounds] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Rounds_Games_GameId] FOREIGN KEY ([GameId]) REFERENCES [dbo].[Games] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [dbo].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909093920_InitialCreate'
)
BEGIN
    CREATE TABLE [dbo].[Answers] (
        [Id] uniqueidentifier NOT NULL,
        [QuestionId] uniqueidentifier NOT NULL,
        [Text] nvarchar(300) NOT NULL,
        [IsCorrect] bit NOT NULL,
        CONSTRAINT [PK_Answers] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Answers_Questions_QuestionId] FOREIGN KEY ([QuestionId]) REFERENCES [dbo].[Questions] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [dbo].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909093920_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Answers_QuestionId] ON [dbo].[Answers] ([QuestionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [dbo].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909093920_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Categories_Name] ON [dbo].[Categories] ([Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [dbo].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909093920_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_Questions_CategoryId_IsActive] ON [dbo].[Questions] ([CategoryId], [IsActive]);
END;

IF NOT EXISTS (
    SELECT * FROM [dbo].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909093920_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Rounds_GameId_Number] ON [dbo].[Rounds] ([GameId], [Number]);
END;

IF NOT EXISTS (
    SELECT * FROM [dbo].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909093920_InitialCreate'
)
BEGIN
    INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260909093920_InitialCreate', N'10.0.0');
END;

COMMIT;
GO


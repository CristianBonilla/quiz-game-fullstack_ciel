CREATE OR ALTER PROCEDURE dbo.sp_GetGameSummary_v1
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

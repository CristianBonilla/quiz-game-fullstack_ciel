CREATE OR ALTER PROCEDURE dbo.sp_GetRandomQuestionByCategory_v1
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
          OR q.Id NOT IN (SELECT CAST(value AS UNIQUEIDENTIFIER) FROM STRING_SPLIT(@ExcludedQuestionIds, ','))
      )
    ORDER BY NEWID();
END

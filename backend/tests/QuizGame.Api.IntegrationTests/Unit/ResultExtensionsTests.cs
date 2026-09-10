using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using QuizGame.Api.Extensions;
using QuizGame.Domain.SeedWork;

namespace QuizGame.Api.IntegrationTests.Unit;

public sealed class ResultExtensionsTests
{
    [Fact]
    public void ToOkResult_Should_ReturnOk200_When_ResultIsSuccess()
    {
        // Arrange
        Result<string> result = Result.Success("payload");

        // Act
        var httpResult = result.ToOkResult();

        // Assert
        Ok<string> ok = httpResult.ShouldBeOfType<Ok<string>>();
        ok.StatusCode.ShouldBe(StatusCodes.Status200OK);
        ok.Value.ShouldBe("payload");
    }

    [Fact]
    public void ToCreatedResult_Should_ReturnCreated201WithLocation_When_ResultIsSuccess()
    {
        // Arrange
        Result<string> result = Result.Success("payload");

        // Act
        var httpResult = result.ToCreatedResult("/api/v1/resource/1");

        // Assert
        Created<string> created = httpResult.ShouldBeOfType<Created<string>>();
        created.StatusCode.ShouldBe(StatusCodes.Status201Created);
        created.Location.ShouldBe("/api/v1/resource/1");
    }

    [Fact]
    public void ToNoContentResult_Should_ReturnNoContent204_When_ResultIsSuccess()
    {
        // Arrange
        Result result = Result.Success();

        // Act
        var httpResult = result.ToNoContentResult();

        // Assert
        httpResult.ShouldBeOfType<NoContent>().StatusCode.ShouldBe(StatusCodes.Status204NoContent);
    }

    [Theory]
    [InlineData(ErrorType.Validation, StatusCodes.Status400BadRequest)]
    [InlineData(ErrorType.NotFound, StatusCodes.Status404NotFound)]
    [InlineData(ErrorType.Conflict, StatusCodes.Status409Conflict)]
    [InlineData(ErrorType.Failure, StatusCodes.Status500InternalServerError)]
    public void ToProblem_Should_MapErrorTypeToExpectedHttpStatusCode(ErrorType errorType, int expectedStatusCode)
    {
        // Arrange
        Error error = new("Some.Code", "Some description", errorType);

        // Act
        ProblemHttpResult problem = error.ToProblem();

        // Assert
        problem.StatusCode.ShouldBe(expectedStatusCode);
        problem.ProblemDetails.Title.ShouldBe(error.Code);
        problem.ProblemDetails.Detail.ShouldBe(error.Description);
    }

    [Fact]
    public void ToOkResult_Should_ReturnProblem_When_ResultIsFailure()
    {
        // Arrange
        Result<string> result = Result.Failure<string>(Error.NotFound("Game.NotFound", "Game not found."));

        // Act
        var httpResult = result.ToOkResult();

        // Assert
        ProblemHttpResult problem = httpResult.ShouldBeOfType<ProblemHttpResult>();
        problem.StatusCode.ShouldBe(StatusCodes.Status404NotFound);
    }
}

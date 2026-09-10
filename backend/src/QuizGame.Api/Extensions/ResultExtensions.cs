using Microsoft.AspNetCore.Http.HttpResults;
using QuizGame.Domain.SeedWork;

namespace QuizGame.Api.Extensions;

public static class ResultExtensions
{
    public static IResult ToOkResult<T>(this Result<T> result) =>
        result.IsSuccess ? TypedResults.Ok(result.Value) : ToProblem(result.Error);

    public static IResult ToCreatedResult<T>(this Result<T> result, string location) =>
        result.IsSuccess ? TypedResults.Created(location, result.Value) : ToProblem(result.Error);

    public static IResult ToNoContentResult(this Result result) =>
        result.IsSuccess ? TypedResults.NoContent() : ToProblem(result.Error);

    public static ProblemHttpResult ToProblem(this Error error)
    {
        int statusCode = error.Type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Failure => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status500InternalServerError
        };

        return TypedResults.Problem(
            title: error.Code,
            detail: error.Description,
            statusCode: statusCode,
            extensions: new Dictionary<string, object?> { ["code"] = error.Code });
    }
}

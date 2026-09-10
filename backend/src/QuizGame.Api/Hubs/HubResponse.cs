using QuizGame.Domain.SeedWork;

namespace QuizGame.Api.Hubs;

public sealed record HubResponse<T>(bool IsSuccess, T? Value, string? ErrorCode, string? ErrorDescription);

public static class HubResultExtensions
{
    public static HubResponse<T> ToHubResponse<T>(this Result<T> result) =>
        result.IsSuccess
            ? new HubResponse<T>(true, result.Value, null, null)
            : new HubResponse<T>(false, default, result.Error.Code, result.Error.Description);
}

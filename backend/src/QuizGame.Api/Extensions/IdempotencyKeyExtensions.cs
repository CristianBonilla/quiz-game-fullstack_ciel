namespace QuizGame.Api.Extensions;

public static class IdempotencyKeyExtensions
{
    public const string HeaderName = "Idempotency-Key";

    /// <summary>Maps the REST header onto the same RequestId the SignalR path uses.</summary>
    public static Guid ResolveIdempotencyKey(this HttpRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        return request.Headers.TryGetValue(HeaderName, out Microsoft.Extensions.Primitives.StringValues header)
            && Guid.TryParse(header.ToString(), out Guid requestId)
                ? requestId
                : Guid.NewGuid();
    }
}

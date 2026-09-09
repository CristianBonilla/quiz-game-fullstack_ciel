using QuizGame.Domain.SeedWork;

namespace QuizGame.Application.Abstractions.Messaging;

public delegate Task<Result<TResponse>> NextHandler<TResponse>();

public interface IPipelineBehavior<in TRequest, TResponse>
{
    Task<Result<TResponse>> HandleAsync(
        TRequest request,
        NextHandler<TResponse> continuation,
        CancellationToken cancellationToken);
}

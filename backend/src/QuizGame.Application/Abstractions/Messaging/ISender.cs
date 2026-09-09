using QuizGame.Domain.SeedWork;

namespace QuizGame.Application.Abstractions.Messaging;

public interface ISender
{
    Task<Result<TResponse>> SendAsync<TResponse>(
        ICommand<TResponse> command,
        CancellationToken cancellationToken);

    Task<Result<TResponse>> QueryAsync<TResponse>(
        IQuery<TResponse> query,
        CancellationToken cancellationToken);
}

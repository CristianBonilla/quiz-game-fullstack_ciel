using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using QuizGame.Domain.SeedWork;

namespace QuizGame.Application.Abstractions.Messaging;

public sealed class Sender(IServiceProvider serviceProvider) : ISender
{
    private static readonly ConcurrentDictionary<Type, object> Dispatchers = new();

    public Task<Result<TResponse>> SendAsync<TResponse>(
        ICommand<TResponse> command,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        CommandDispatcher<TResponse> dispatcher = (CommandDispatcher<TResponse>)Dispatchers.GetOrAdd(
            command.GetType(),
            static commandType => Activator.CreateInstance(
                typeof(CommandDispatcherImplementation<,>).MakeGenericType(commandType, typeof(TResponse)))!);

        return dispatcher.DispatchAsync(command, serviceProvider, cancellationToken);
    }

    public Task<Result<TResponse>> QueryAsync<TResponse>(
        IQuery<TResponse> query,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(query);

        QueryDispatcher<TResponse> dispatcher = (QueryDispatcher<TResponse>)Dispatchers.GetOrAdd(
            query.GetType(),
            static queryType => Activator.CreateInstance(
                typeof(QueryDispatcherImplementation<,>).MakeGenericType(queryType, typeof(TResponse)))!);

        return dispatcher.DispatchAsync(query, serviceProvider, cancellationToken);
    }

    private abstract class CommandDispatcher<TResponse>
    {
        public abstract Task<Result<TResponse>> DispatchAsync(
            object command,
            IServiceProvider serviceProvider,
            CancellationToken cancellationToken);
    }

    private sealed class CommandDispatcherImplementation<TCommand, TResponse> : CommandDispatcher<TResponse>
        where TCommand : ICommand<TResponse>
    {
        public override Task<Result<TResponse>> DispatchAsync(
            object command,
            IServiceProvider serviceProvider,
            CancellationToken cancellationToken)
        {
            TCommand typedCommand = (TCommand)command;
            ICommandHandler<TCommand, TResponse> handler =
                serviceProvider.GetRequiredService<ICommandHandler<TCommand, TResponse>>();

            return PipelineComposer.Compose(
                typedCommand,
                () => handler.HandleAsync(typedCommand, cancellationToken),
                serviceProvider,
                cancellationToken);
        }
    }

    private abstract class QueryDispatcher<TResponse>
    {
        public abstract Task<Result<TResponse>> DispatchAsync(
            object query,
            IServiceProvider serviceProvider,
            CancellationToken cancellationToken);
    }

    private sealed class QueryDispatcherImplementation<TQuery, TResponse> : QueryDispatcher<TResponse>
        where TQuery : IQuery<TResponse>
    {
        public override Task<Result<TResponse>> DispatchAsync(
            object query,
            IServiceProvider serviceProvider,
            CancellationToken cancellationToken)
        {
            TQuery typedQuery = (TQuery)query;
            IQueryHandler<TQuery, TResponse> handler =
                serviceProvider.GetRequiredService<IQueryHandler<TQuery, TResponse>>();

            return PipelineComposer.Compose(
                typedQuery,
                () => handler.HandleAsync(typedQuery, cancellationToken),
                serviceProvider,
                cancellationToken);
        }
    }

    private static class PipelineComposer
    {
        public static Task<Result<TResponse>> Compose<TRequest, TResponse>(
            TRequest request,
            NextHandler<TResponse> handler,
            IServiceProvider serviceProvider,
            CancellationToken cancellationToken)
        {
            NextHandler<TResponse> pipeline = handler;

            IPipelineBehavior<TRequest, TResponse>[] behaviors =
                [.. serviceProvider.GetServices<IPipelineBehavior<TRequest, TResponse>>()];

            for (int index = behaviors.Length - 1; index >= 0; index--)
            {
                IPipelineBehavior<TRequest, TResponse> behavior = behaviors[index];
                NextHandler<TResponse> continuation = pipeline;
                pipeline = () => behavior.HandleAsync(request, continuation, cancellationToken);
            }

            return pipeline();
        }
    }
}

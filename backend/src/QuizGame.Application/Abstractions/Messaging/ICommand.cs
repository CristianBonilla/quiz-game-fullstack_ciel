namespace QuizGame.Application.Abstractions.Messaging;

#pragma warning disable CA1040 // Marker interfaces: they carry dispatch semantics, not members.
public interface IBaseCommand
{
}

public interface ICommand<TResponse> : IBaseCommand
{
}

public interface ICommand : ICommand<Unit>
{
}

public interface IQuery<TResponse>
{
}
#pragma warning restore CA1040

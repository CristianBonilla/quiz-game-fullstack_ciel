namespace QuizGame.Application.Abstractions.Messaging;

public interface IIdempotentCommand
{
    Guid RequestId { get; }
}

namespace QuizGame.Infrastructure.Resilience;

/// <summary>Marks a failure the outbox dispatcher may safely retry.</summary>
public sealed class TransientDispatchException : Exception
{
    public TransientDispatchException()
    {
    }

    public TransientDispatchException(string message) : base(message)
    {
    }

    public TransientDispatchException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

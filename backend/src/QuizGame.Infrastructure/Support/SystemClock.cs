using QuizGame.Application.Abstractions;

namespace QuizGame.Infrastructure.Support;

public sealed class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}

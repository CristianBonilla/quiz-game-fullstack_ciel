using QuizGame.Application.Abstractions;

namespace QuizGame.Infrastructure.Support;

public sealed class RandomProvider : IRandomProvider
{
    public int NextInt32(int maxExclusive) => Random.Shared.Next(maxExclusive);

    public T Pick<T>(IReadOnlyList<T> source)
    {
        ArgumentNullException.ThrowIfNull(source);

        if (source.Count == 0)
        {
            throw new ArgumentException("The source collection cannot be empty.", nameof(source));
        }

        return source[Random.Shared.Next(source.Count)];
    }
}

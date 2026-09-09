namespace QuizGame.Application.Abstractions;

public interface IRandomProvider
{
    int NextInt32(int maxExclusive);

    T Pick<T>(IReadOnlyList<T> source);
}

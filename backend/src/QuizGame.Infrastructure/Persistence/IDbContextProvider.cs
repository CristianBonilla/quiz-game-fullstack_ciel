namespace QuizGame.Infrastructure.Persistence;

public interface IDbContextProvider
{
    QuizGameDbContext? CurrentContext { get; }

    QuizGameDbContext GetContext();
}

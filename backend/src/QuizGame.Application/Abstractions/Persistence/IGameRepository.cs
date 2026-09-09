using QuizGame.Domain.Games;
using QuizGame.Domain.SeedWork;

namespace QuizGame.Application.Abstractions.Persistence;

public interface IGameRepository : IRepository<Game>
{
    Task<Game?> GetByIdAsync(Guid gameId, CancellationToken cancellationToken);

    void Add(Game game);
}

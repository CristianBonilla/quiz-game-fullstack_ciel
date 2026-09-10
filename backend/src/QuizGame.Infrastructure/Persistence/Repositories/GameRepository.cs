using Microsoft.EntityFrameworkCore;
using QuizGame.Application.Abstractions.Persistence;
using QuizGame.Domain.Games;

namespace QuizGame.Infrastructure.Persistence.Repositories;

public sealed class GameRepository(IDbContextProvider contextProvider) : IGameRepository
{
    public async Task<Game?> GetByIdAsync(Guid gameId, CancellationToken cancellationToken)
    {
        QuizGameDbContext context = contextProvider.GetContext();

        return await context.Games
            .Include(game => game.Rounds)
            .FirstOrDefaultAsync(game => game.Id == gameId, cancellationToken)
            .ConfigureAwait(false);
    }

    public void Add(Game game)
    {
        ArgumentNullException.ThrowIfNull(game);

        contextProvider.GetContext().Games.Add(game);
    }
}

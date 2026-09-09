using QuizGame.Domain.Categories;
using QuizGame.Domain.SeedWork;

namespace QuizGame.Application.Abstractions.Persistence;

public interface ICategoryRepository : IRepository<Category>
{
    Task<Category?> GetByIdAsync(Guid categoryId, CancellationToken cancellationToken);

    Task<Category?> GetActiveByDifficultyAsync(int difficultyLevel, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Category>> ListAsync(bool onlyActive, CancellationToken cancellationToken);

    Task<bool> NameExistsAsync(string name, Guid? excludedCategoryId, CancellationToken cancellationToken);

    void Add(Category category);
}

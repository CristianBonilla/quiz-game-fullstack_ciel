using Microsoft.EntityFrameworkCore;
using QuizGame.Application.Abstractions.Persistence;
using QuizGame.Domain.Categories;
using QuizGame.Domain.SeedWork;
using QuizGame.Domain.ValueObjects;
using QuizGame.Infrastructure.Persistence.Specifications;

namespace QuizGame.Infrastructure.Persistence.Repositories;

public sealed class CategoryRepository(IDbContextProvider contextProvider) : ICategoryRepository
{
    public async Task<Category?> GetByIdAsync(Guid categoryId, CancellationToken cancellationToken)
    {
        QuizGameDbContext context = contextProvider.GetContext();

        return await context.Categories
            .Include(category => category.Questions)
            .ThenInclude(question => question.Answers)
            .FirstOrDefaultAsync(category => category.Id == categoryId, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<Category?> GetActiveByDifficultyAsync(int difficultyLevel, CancellationToken cancellationToken)
    {
        QuizGameDbContext context = contextProvider.GetContext();

        Result<DifficultyLevel> level = DifficultyLevel.Create(difficultyLevel);
        if (level.IsSuccess)
        {
            Category? exactMatch = await context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    category => category.IsActive && category.DifficultyLevel == level.Value,
                    cancellationToken)
                .ConfigureAwait(false);

            if (exactMatch is not null)
            {
                return exactMatch;
            }
        }

        List<Category> active = await context.Categories
            .AsNoTracking()
            .Where(category => category.IsActive)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        if (active.Count == 0)
        {
            return null;
        }

        return active
            .OrderBy(category => Math.Abs(category.DifficultyLevel.Value - difficultyLevel))
            .ThenByDescending(category => category.DifficultyLevel.Value)
            .FirstOrDefault();
    }

    public async Task<IReadOnlyCollection<Category>> ListAsync(bool onlyActive, CancellationToken cancellationToken)
    {
        QuizGameDbContext context = contextProvider.GetContext();

        IQueryable<Category> query = SpecificationEvaluator.Apply(
            context.Categories,
            new CategoriesSpecification(onlyActive));

        return await query.ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<bool> NameExistsAsync(string name, Guid? excludedCategoryId, CancellationToken cancellationToken)
    {
        QuizGameDbContext context = contextProvider.GetContext();

        return await context.Categories
            .AsNoTracking()
            .AnyAsync(
                category => category.Name == name
                    && (!excludedCategoryId.HasValue || category.Id != excludedCategoryId.Value),
                cancellationToken)
            .ConfigureAwait(false);
    }

    public void Add(Category category)
    {
        ArgumentNullException.ThrowIfNull(category);

        contextProvider.GetContext().Categories.Add(category);
    }
}

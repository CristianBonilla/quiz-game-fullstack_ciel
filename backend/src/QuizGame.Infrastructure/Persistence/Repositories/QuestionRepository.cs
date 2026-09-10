using Microsoft.EntityFrameworkCore;
using QuizGame.Application.Abstractions.Persistence;
using QuizGame.Domain.Games;
using QuizGame.Domain.Questions;
using QuizGame.Infrastructure.Persistence.Specifications;

namespace QuizGame.Infrastructure.Persistence.Repositories;

public sealed class QuestionRepository(IDbContextProvider contextProvider) : IQuestionRepository
{
    public async Task<Question?> GetByIdAsync(Guid questionId, CancellationToken cancellationToken)
    {
        QuizGameDbContext context = contextProvider.GetContext();

        return await context.Questions
            .Include(question => question.Answers)
            .FirstOrDefaultAsync(question => question.Id == questionId, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<IReadOnlyCollection<Question>> ListByCategoryAsync(
        Guid categoryId,
        bool onlyActive,
        CancellationToken cancellationToken)
    {
        QuizGameDbContext context = contextProvider.GetContext();

        IQueryable<Question> query = context.Questions
            .AsNoTracking()
            .Include(question => question.Answers)
            .Where(question => question.CategoryId == categoryId);

        if (onlyActive)
        {
            query = query.Where(question => question.IsActive);
        }

        return await query.ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyCollection<Question>> ListSelectableAsync(
        Guid categoryId,
        IReadOnlyCollection<Guid> excludedQuestionIds,
        CancellationToken cancellationToken)
    {
        QuizGameDbContext context = contextProvider.GetContext();

        IQueryable<Question> query = SpecificationEvaluator.Apply(
            context.Questions,
            new SelectableQuestionsSpecification(categoryId, excludedQuestionIds));

        return await query.ToListAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<IReadOnlyDictionary<Guid, int>> GetUsageCountsAsync(
        IReadOnlyCollection<Guid> questionIds,
        CancellationToken cancellationToken)
    {
        if (questionIds.Count == 0)
        {
            return new Dictionary<Guid, int>();
        }

        QuizGameDbContext context = contextProvider.GetContext();

        List<QuestionUsage> usages = await context.Set<Round>()
            .AsNoTracking()
            .Where(round => questionIds.Contains(round.QuestionId))
            .GroupBy(round => round.QuestionId)
            .Select(group => new QuestionUsage(group.Key, group.Count()))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return usages.ToDictionary(usage => usage.QuestionId, usage => usage.Count);
    }

    public async Task<int> CountActiveByCategoryAsync(Guid categoryId, CancellationToken cancellationToken)
    {
        QuizGameDbContext context = contextProvider.GetContext();

        return await context.Questions
            .AsNoTracking()
            .CountAsync(question => question.CategoryId == categoryId && question.IsActive, cancellationToken)
            .ConfigureAwait(false);
    }

    public void Add(Question question)
    {
        ArgumentNullException.ThrowIfNull(question);

        contextProvider.GetContext().Questions.Add(question);
    }

    public async Task<Question?> GetRandomViaStoredProcedureAsync(
        Guid categoryId,
        IReadOnlyCollection<Guid> excludedQuestionIds,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(excludedQuestionIds);

        QuizGameDbContext context = contextProvider.GetContext();
        string? excluded = excludedQuestionIds.Count == 0 ? null : string.Join(',', excludedQuestionIds);

        return await context.Questions
            .FromSqlInterpolated(
                $"EXEC dbo.sp_GetRandomQuestionByCategory_v1 @CategoryId = {categoryId}, @ExcludedQuestionIds = {excluded}")
            .Include(question => question.Answers)
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    private sealed record QuestionUsage(Guid QuestionId, int Count);
}

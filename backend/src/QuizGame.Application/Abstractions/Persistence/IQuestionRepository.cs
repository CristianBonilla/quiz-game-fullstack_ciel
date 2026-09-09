using QuizGame.Domain.Questions;
using QuizGame.Domain.SeedWork;

namespace QuizGame.Application.Abstractions.Persistence;

public interface IQuestionRepository : IRepository<Question>
{
    Task<Question?> GetByIdAsync(Guid questionId, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Question>> ListByCategoryAsync(
        Guid categoryId,
        bool onlyActive,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Question>> ListSelectableAsync(
        Guid categoryId,
        IReadOnlyCollection<Guid> excludedQuestionIds,
        CancellationToken cancellationToken);

    Task<IReadOnlyDictionary<Guid, int>> GetUsageCountsAsync(
        IReadOnlyCollection<Guid> questionIds,
        CancellationToken cancellationToken);

    Task<int> CountActiveByCategoryAsync(Guid categoryId, CancellationToken cancellationToken);

    void Add(Question question);
}

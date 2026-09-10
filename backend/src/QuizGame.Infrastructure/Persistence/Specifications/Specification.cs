using System.Linq.Expressions;
using QuizGame.Domain.SeedWork;

namespace QuizGame.Infrastructure.Persistence.Specifications;

public abstract class Specification<T> : ISpecification<T>
{
    private readonly List<Expression<Func<T, object>>> _includes = [];

    protected Specification(Expression<Func<T, bool>>? criteria) => Criteria = criteria;

    public Expression<Func<T, bool>>? Criteria { get; }

    public IReadOnlyCollection<Expression<Func<T, object>>> Includes => _includes;

    public Expression<Func<T, object>>? OrderBy { get; protected set; }

    public Expression<Func<T, object>>? OrderByDescending { get; protected set; }

    public bool AsNoTracking { get; protected init; } = true;

    protected void AddInclude(Expression<Func<T, object>> include) => _includes.Add(include);
}

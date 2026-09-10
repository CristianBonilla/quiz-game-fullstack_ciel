using Microsoft.EntityFrameworkCore;
using QuizGame.Domain.SeedWork;

namespace QuizGame.Infrastructure.Persistence.Specifications;

public static class SpecificationEvaluator
{
    public static IQueryable<T> Apply<T>(IQueryable<T> source, ISpecification<T> specification)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(specification);

        IQueryable<T> query = specification.Criteria is null
            ? source
            : source.Where(specification.Criteria);

        query = specification.Includes.Aggregate(query, (current, include) => current.Include(include));

        if (specification.OrderBy is not null)
        {
            query = query.OrderBy(specification.OrderBy);
        }
        else if (specification.OrderByDescending is not null)
        {
            query = query.OrderByDescending(specification.OrderByDescending);
        }

        return specification.AsNoTracking ? query.AsNoTracking() : query;
    }
}

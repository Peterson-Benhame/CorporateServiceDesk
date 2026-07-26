using CorporateServiceDesk.Application.Common.Specifications;

namespace CorporateServiceDesk.Infrastructure.Specifications;

public static class SpecificationEvaluator
{
    public static IQueryable<TEntity> Apply<TEntity>(
        IQueryable<TEntity> query,
        ISpecification<TEntity> specification) =>
        specification.Criteria is null
            ? query
            : query.Where(specification.Criteria);
}

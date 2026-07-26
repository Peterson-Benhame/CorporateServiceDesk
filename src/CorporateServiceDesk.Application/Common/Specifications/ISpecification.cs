using System.Linq.Expressions;

namespace CorporateServiceDesk.Application.Common.Specifications;

public interface ISpecification<TEntity>
{
    Expression<Func<TEntity, bool>>? Criteria { get; }
}

using System.Linq.Expressions;

namespace CorporateServiceDesk.Application.Common.Specifications;

public abstract class Specification<TEntity> : ISpecification<TEntity>
{
    public Expression<Func<TEntity, bool>>? Criteria { get; private set; }

    protected void AddAnd(Expression<Func<TEntity, bool>> criterion) =>
        Criteria = Criteria is null ? criterion : Combine(Criteria, criterion, Expression.AndAlso);

    protected void AddOr(Expression<Func<TEntity, bool>> criterion) =>
        Criteria = Criteria is null ? criterion : Combine(Criteria, criterion, Expression.OrElse);

    protected void SetCriteria(Expression<Func<TEntity, bool>>? criterion) =>
        Criteria = criterion;

    private static Expression<Func<TEntity, bool>> Combine(
        Expression<Func<TEntity, bool>> left,
        Expression<Func<TEntity, bool>> right,
        Func<Expression, Expression, BinaryExpression> merge)
    {
        var parameter = Expression.Parameter(typeof(TEntity), "entity");
        var leftBody = new ReplaceParameterVisitor(left.Parameters[0], parameter).Visit(left.Body)!;
        var rightBody = new ReplaceParameterVisitor(right.Parameters[0], parameter).Visit(right.Body)!;
        return Expression.Lambda<Func<TEntity, bool>>(merge(leftBody, rightBody), parameter);
    }

    private sealed class ReplaceParameterVisitor(
        ParameterExpression source,
        ParameterExpression target) : ExpressionVisitor
    {
        protected override Expression VisitParameter(ParameterExpression node) =>
            node == source ? target : base.VisitParameter(node);
    }
}

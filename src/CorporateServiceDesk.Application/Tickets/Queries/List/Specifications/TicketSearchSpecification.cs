using CorporateServiceDesk.Application.Common.Specifications;
using CorporateServiceDesk.Domain.Tickets.Entities;
using CorporateServiceDesk.Domain.Tickets.Enums;
using System.Globalization;
using System.Linq.Expressions;

namespace CorporateServiceDesk.Application.Tickets.Queries.List.Specifications;

public sealed class TicketSearchSpecification : Specification<Ticket>
{
    public TicketSearchSpecification(IReadOnlyList<TicketFilterCriterion> criteria) =>
        SetCriteria(Build(criteria));

    private static Expression<Func<Ticket, bool>>? Build(IReadOnlyList<TicketFilterCriterion> criteria)
    {
        if (criteria.Count == 0) return null;
        var parameter = Expression.Parameter(typeof(Ticket), "ticket");
        Expression? completed = null;
        Expression? group = null;

        for (var index = 0; index < criteria.Count; index++)
        {
            var current = BuildCriterion(parameter, criteria[index]);
            if (group is null) { group = current; continue; }
            if (criteria[index].LogicalOperator == LogicalOperator.Or)
            {
                group = Expression.OrElse(group, current);
                continue;
            }
            completed = completed is null ? group : Expression.AndAlso(completed, group);
            group = current;
        }

        var body = completed is null ? group! : Expression.AndAlso(completed, group!);
        return Expression.Lambda<Func<Ticket, bool>>(body, parameter);
    }

    private static Expression BuildCriterion(ParameterExpression parameter, TicketFilterCriterion criterion)
    {
        var property = Expression.Property(parameter, criterion.Column.ToString());
        if (criterion.Operator == FilterOperator.IsNull)
            return Expression.Equal(property, Expression.Constant(null, property.Type));
        if (criterion.Operator == FilterOperator.IsNotNull)
            return Expression.NotEqual(property, Expression.Constant(null, property.Type));

        var value = ParseValue(criterion);
        var underlying = Nullable.GetUnderlyingType(property.Type);
        var constant = underlying is null
            ? Expression.Constant(value, property.Type)
            : Expression.Constant(Activator.CreateInstance(property.Type, value), property.Type);

        return criterion.Operator switch
        {
            FilterOperator.Equals => Expression.Equal(property, constant),
            FilterOperator.NotEquals => Expression.NotEqual(property, constant),
            FilterOperator.GreaterThan => Expression.GreaterThan(property, constant),
            FilterOperator.GreaterThanOrEqual => Expression.GreaterThanOrEqual(property, constant),
            FilterOperator.LessThan => Expression.LessThan(property, constant),
            FilterOperator.LessThanOrEqual => Expression.LessThanOrEqual(property, constant),
            FilterOperator.Contains => Expression.Call(property, nameof(string.Contains), Type.EmptyTypes, constant),
            _ => throw new InvalidOperationException($"Unsupported operator {criterion.Operator}.")
        };
    }

    private static object ParseValue(TicketFilterCriterion criterion) => criterion.Column switch
    {
        TicketFilterColumn.Status => Enum.Parse<TicketStatus>(criterion.Value!, true),
        TicketFilterColumn.Priority => Enum.Parse<TicketPriority>(criterion.Value!, true),
        TicketFilterColumn.RequesterId or TicketFilterColumn.AssigneeId => Guid.Parse(criterion.Value!),
        TicketFilterColumn.OpenedAtUtc or TicketFilterColumn.ClosedAtUtc =>
            DateTimeOffset.Parse(criterion.Value!, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
        _ => criterion.Value!
    };
}

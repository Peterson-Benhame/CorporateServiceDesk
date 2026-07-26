using CorporateServiceDesk.Domain.Tickets.Enums;
using FluentValidation;
using System.Globalization;

namespace CorporateServiceDesk.Application.Tickets.Queries.List.Validation;

public sealed class TicketFilterCriterionValidator : AbstractValidator<TicketFilterCriterion>
{
    public TicketFilterCriterionValidator()
    {
        RuleFor(x => x.Column).IsInEnum();
        RuleFor(x => x.Operator).IsInEnum();
        RuleFor(x => x.LogicalOperator).IsInEnum();
        RuleFor(x => x).Custom(ValidateCriterion);
    }

    private static void ValidateCriterion(
        TicketFilterCriterion criterion,
        ValidationContext<TicketFilterCriterion> context)
    {
        var allowed = criterion.Column switch
        {
            TicketFilterColumn.Title => criterion.Operator is FilterOperator.Equals or FilterOperator.NotEquals or FilterOperator.Contains,
            TicketFilterColumn.Status or TicketFilterColumn.Priority or TicketFilterColumn.RequesterId =>
                criterion.Operator is FilterOperator.Equals or FilterOperator.NotEquals,
            TicketFilterColumn.AssigneeId => criterion.Operator is FilterOperator.Equals or FilterOperator.NotEquals or FilterOperator.IsNull or FilterOperator.IsNotNull,
            TicketFilterColumn.OpenedAtUtc => IsComparison(criterion.Operator),
            TicketFilterColumn.ClosedAtUtc => IsComparison(criterion.Operator) || criterion.Operator is FilterOperator.IsNull or FilterOperator.IsNotNull,
            _ => false
        };

        if (!allowed)
        {
            context.AddFailure(nameof(criterion.Operator), $"Operator {criterion.Operator} is not valid for {criterion.Column}.");
            return;
        }

        if (criterion.Operator is FilterOperator.IsNull or FilterOperator.IsNotNull) return;
        if (string.IsNullOrWhiteSpace(criterion.Value))
        {
            context.AddFailure(nameof(criterion.Value), "A value is required for this operator.");
            return;
        }

        var valid = criterion.Column switch
        {
            TicketFilterColumn.Status => TryDefinedEnum<TicketStatus>(criterion.Value),
            TicketFilterColumn.Priority => TryDefinedEnum<TicketPriority>(criterion.Value),
            TicketFilterColumn.RequesterId or TicketFilterColumn.AssigneeId => Guid.TryParse(criterion.Value, out _),
            TicketFilterColumn.OpenedAtUtc or TicketFilterColumn.ClosedAtUtc =>
                DateTimeOffset.TryParse(criterion.Value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out _),
            _ => true
        };

        if (!valid) context.AddFailure(nameof(criterion.Value), $"Value is invalid for {criterion.Column}.");
    }

    private static bool IsComparison(FilterOperator value) =>
        value is FilterOperator.Equals or FilterOperator.NotEquals or FilterOperator.GreaterThan or
            FilterOperator.GreaterThanOrEqual or FilterOperator.LessThan or FilterOperator.LessThanOrEqual;

    private static bool TryDefinedEnum<TEnum>(string value) where TEnum : struct, Enum =>
        Enum.TryParse<TEnum>(value, true, out var parsed) && Enum.IsDefined(parsed);
}

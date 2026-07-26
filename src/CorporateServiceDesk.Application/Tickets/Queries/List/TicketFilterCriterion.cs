namespace CorporateServiceDesk.Application.Tickets.Queries.List;

public sealed record TicketFilterCriterion(
    TicketFilterColumn Column,
    FilterOperator Operator,
    string? Value,
    LogicalOperator LogicalOperator = LogicalOperator.And);

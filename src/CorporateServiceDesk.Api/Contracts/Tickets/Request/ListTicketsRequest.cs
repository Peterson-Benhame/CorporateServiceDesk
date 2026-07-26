using CorporateServiceDesk.Application.Common.Pagination;
using CorporateServiceDesk.Application.Tickets.Queries.List;
using CorporateServiceDesk.Domain.Tickets.Enums;
using System.ComponentModel.DataAnnotations;

namespace CorporateServiceDesk.Api.Contracts.Tickets.Request;

public sealed class ListTicketsRequest
{
    public int Page { get; init; } = PageRequest.DefaultPage;
    public int PageSize { get; init; } = PageRequest.DefaultPageSize;
    public bool CountTotal { get; init; }
    public TicketSortColumn? SortBy { get; init; }
    public SortDirection SortDirection { get; init; } = SortDirection.Descending;
    public TicketStatus? Status { get; init; }
    public TicketPriority? Priority { get; init; }
    public Guid? RequesterId { get; init; }
    public Guid? AssigneeId { get; init; }
    public DateTimeOffset? OpenedFromUtc { get; init; }
    public DateTimeOffset? OpenedToUtc { get; init; }
    public string? Search { get; init; }

    internal QueryListTicketsFilter Map() =>
        new(
            new PageRequest(
                Page,
                PageSize,
                CountTotal),
            SortBy.HasValue ? new SortRequest<TicketSortColumn>(SortBy.Value, SortDirection) : null,
            Status,
            Priority,
            RequesterId,
            AssigneeId,
            OpenedFromUtc,
            OpenedToUtc,
            Search);
}

public sealed class SearchTicketsRequest
{
    public int Page { get; init; } = PageRequest.DefaultPage;
    public int PageSize { get; init; } = PageRequest.DefaultPageSize;
    public bool CountTotal { get; init; }
    public TicketSortColumn? SortBy { get; init; }
    public SortDirection SortDirection { get; init; } = SortDirection.Descending;
    [Required]
    public IReadOnlyList<TicketFilterCriterionRequest> Criteria { get; init; } =
        Array.Empty<TicketFilterCriterionRequest>();

    internal QuerySearchTicketsFilter Map() =>
        new(
            new PageRequest(
                Page,
                PageSize,
                CountTotal),
            SortBy.HasValue ? new SortRequest<TicketSortColumn>(SortBy.Value, SortDirection) : null,
            Criteria.Select(criterion => criterion.Map()).ToList());
}

public sealed record TicketFilterCriterionRequest(
    TicketFilterColumn Column,
    FilterOperator Operator,
    string? Value,
    LogicalOperator LogicalOperator = LogicalOperator.And)
{
    internal TicketFilterCriterion Map() =>
        new(Column, Operator, Value, LogicalOperator);
}

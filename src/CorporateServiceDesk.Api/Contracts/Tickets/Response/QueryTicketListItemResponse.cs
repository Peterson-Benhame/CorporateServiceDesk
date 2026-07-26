using CorporateServiceDesk.Application.Tickets.Queries.List;
using CorporateServiceDesk.Domain.Tickets.Enums;

namespace CorporateServiceDesk.Api.Contracts.Tickets.Response;

public sealed record QueryTicketListItemResponse(
    Guid Id,
    string Title,
    Guid RequesterId,
    Guid? AssigneeId,
    TicketPriority Priority,
    TicketStatus Status,
    DateTimeOffset OpenedAtUtc,
    DateTimeOffset? ClosedAtUtc);

internal static class QueryTicketListItemResponseMapper
{
    public static QueryTicketListItemResponse Map(
        QueryTicketListItemResult result) =>
        new(
            result.Id,
            result.Title,
            result.RequesterId,
            result.AssigneeId,
            result.Priority,
            result.Status,
            result.OpenedAtUtc,
            result.ClosedAtUtc);
}

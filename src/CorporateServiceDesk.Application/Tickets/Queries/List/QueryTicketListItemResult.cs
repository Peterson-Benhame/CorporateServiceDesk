using CorporateServiceDesk.Domain.Tickets.Enums;

namespace CorporateServiceDesk.Application.Tickets.Queries.List;

public sealed record QueryTicketListItemResult(
    Guid Id,
    string Title,
    Guid RequesterId,
    Guid? AssigneeId,
    TicketPriority Priority,
    TicketStatus Status,
    DateTimeOffset OpenedAtUtc,
    DateTimeOffset? ClosedAtUtc);

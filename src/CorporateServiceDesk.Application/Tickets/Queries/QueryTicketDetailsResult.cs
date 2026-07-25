using CorporateServiceDesk.Domain.Tickets.Enums;

namespace CorporateServiceDesk.Application.Tickets.Queries
{
    public sealed record QueryTicketDetailsResult(
                    Guid Id,
                    string Title,
                    string Description,
                    Guid RequesterId,
                    Guid? AssigneeId,
                    TicketPriority Priority,
                    TicketStatus Status,
                    DateTimeOffset OpenedAtUtc,
                    DateTimeOffset? ClosedAtUtc);
}

using CorporateServiceDesk.Domain.Tickets.Enums;

namespace CorporateServiceDesk.Application.Tickets.Create
{
    public sealed record CreateTicketResult(
    Guid Id,
    string Title,
    TicketStatus Status,
    DateTimeOffset OpenedAtUtc);

}

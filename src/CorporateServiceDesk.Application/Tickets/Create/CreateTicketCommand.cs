using CorporateServiceDesk.Domain.Tickets.Enums;

namespace CorporateServiceDesk.Application.Tickets.Create
{
    public sealed record CreateTicketCommand(
    string Title,
    string Description,
    Guid RequesterId,
    TicketPriority Priority);
}

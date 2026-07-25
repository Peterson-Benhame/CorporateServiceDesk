using CorporateServiceDesk.Api.Contracts.Tickets.Request;
using CorporateServiceDesk.Application.Tickets.Create;
using CorporateServiceDesk.Domain.Tickets.Enums;

namespace CorporateServiceDesk.Api.Contracts.Tickets.Response
{
    public sealed record CreateTicketResponse(
            Guid Id,
            string Title,
            string Description,
            Guid RequesterId,
            TicketPriority Priority,
            TicketStatus Status,
            DateTimeOffset OpenedAtUtc);
    internal static class CreateTicketResponseMapper
    {
        public static CreateTicketResponse Map(CreateTicketResult result)
        {
            return new CreateTicketResponse(
                result.Id,
                result.Title,
                result.Description,
                result.RequesterId,
                result.Priority,
                result.Status,
                result.OpenedAtUtc);
        }
    }
}
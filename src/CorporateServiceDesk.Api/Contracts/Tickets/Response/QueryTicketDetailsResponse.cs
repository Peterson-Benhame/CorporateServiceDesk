using CorporateServiceDesk.Application.Tickets.Create;
using CorporateServiceDesk.Application.Tickets.Queries;
using CorporateServiceDesk.Domain.Tickets.Entities;
using CorporateServiceDesk.Domain.Tickets.Enums;

namespace CorporateServiceDesk.Api.Contracts.Tickets.Response
{
    public sealed record QueryTicketDetailsResponse(
                    Guid Id,
                    string Title,
                    string Description,
                    Guid RequesterId,
                    Guid? AssigneeId,
                    TicketPriority Priority,
                    TicketStatus Status,
                    DateTimeOffset OpenedAtUtc,
                    DateTimeOffset? ClosedAtUtc);
    internal static class QueryTicketDetailsResponseMapper
    {
        public static QueryTicketDetailsResponse Map(QueryTicketDetailsResult result)
        {
            return new QueryTicketDetailsResponse(
                                        result.Id,
                                        result.Title,
                                        result.Description,
                                        result.RequesterId,
                                        result.AssigneeId,
                                        result.Priority,
                                        result.Status,
                                        result.OpenedAtUtc,
                                        result.ClosedAtUtc);
        }
    }
}

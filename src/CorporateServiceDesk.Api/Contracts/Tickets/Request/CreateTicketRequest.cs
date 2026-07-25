using CorporateServiceDesk.Domain.Tickets.Enums;
using System.ComponentModel.DataAnnotations;

namespace CorporateServiceDesk.Api.Contracts.Tickets.Request
{
    public sealed record CreateTicketRequest(
    [Required, StringLength(160, MinimumLength = 3)] string Title,
    [Required, StringLength(4000, MinimumLength = 10)] string Description,
    [Required] Guid RequesterId,
    TicketPriority Priority);
}

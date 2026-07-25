using CorporateServiceDesk.Application.Common.Abstractions;
using CorporateServiceDesk.Application.Common.Exceptions;
using CorporateServiceDesk.Application.Tickets.Abstractions;

namespace CorporateServiceDesk.Application.Tickets.Queries
{
    public sealed class QueryGetTicketByIdUseCase(ITicketRepository queries) : IUseCase
    {
        public async Task<QueryTicketDetailsResult> ExecuteAsync(Guid id, CancellationToken cancellationToken)
        {
            if (id == Guid.Empty)
            {
                throw new NotFoundException("Ticket was not found.");
            }
            var ticket = await queries.GetByIdAsync(id, cancellationToken);

            if (ticket == null)
            {
                throw new NotFoundException("Ticket was not found.");
            }

            return new QueryTicketDetailsResult(
                                        ticket.Id,
                                        ticket.Title,
                                        ticket.Description,
                                        ticket.RequesterId,
                                        ticket.AssigneeId,
                                        ticket.Priority,
                                        ticket.Status,
                                        ticket.OpenedAtUtc,
                                        ticket.ClosedAtUtc);
        }
    }
}

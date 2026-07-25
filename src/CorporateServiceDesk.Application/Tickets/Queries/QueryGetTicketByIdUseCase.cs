using CorporateServiceDesk.Application.Common.Abstractions;
using CorporateServiceDesk.Application.Common.Abstractions.Notifications;
using CorporateServiceDesk.Application.Common.Exceptions;
using CorporateServiceDesk.Application.Tickets.Abstractions;
using CorporateServiceDesk.Application.Tickets.Create;

namespace CorporateServiceDesk.Application.Tickets.Queries
{
    public sealed class QueryGetTicketByIdUseCase(ITicketRepository queries) : IUseCase
    {
        public async Task<Result<QueryTicketDetailsResult>> ExecuteAsync(Guid id, CancellationToken cancellationToken)
        {
            var ticket = await queries.GetByIdAsync(id, cancellationToken);

            if (ticket == null)
            {
                return Result<QueryTicketDetailsResult>.Failure("Ticket was not found.", EnumErrorType.NotFound);
            }

            return Result<QueryTicketDetailsResult>.Success(new QueryTicketDetailsResult(
                                                                            ticket.Id,
                                                                            ticket.Title,
                                                                            ticket.Description,
                                                                            ticket.RequesterId,
                                                                            ticket.AssigneeId,
                                                                            ticket.Priority,
                                                                            ticket.Status,
                                                                            ticket.OpenedAtUtc,
                                                                            ticket.ClosedAtUtc));
        }
    }
}


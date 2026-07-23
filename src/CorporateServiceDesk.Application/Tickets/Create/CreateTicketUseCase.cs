using CorporateServiceDesk.Application.Common.Abstractions.Persistence;
using CorporateServiceDesk.Application.Common.Exceptions;
using CorporateServiceDesk.Application.Tickets.Abstractions;
using CorporateServiceDesk.Domain.Tickets.Entities;

namespace CorporateServiceDesk.Application.Tickets.Create
{
    public sealed class CreateTicketUseCase(ITicketRepository ticketRepository, IUnitOfWork unitOfWork, TimeProvider timeProvider)
    {
        public async Task<CreateTicketResult> ExecuteAsync(CreateTicketCommand command, CancellationToken cancellationToken)
        {
            var normalizedTitle = command.Title.Trim();
            var duplicateExists = await ticketRepository.ExistsByTitleForRequesterAsync(command.RequesterId, normalizedTitle, cancellationToken);

            if (duplicateExists)
            {
                throw new ConflictException("A similar open ticket already exists for this requester.");
            }

            var ticket = Ticket.Open(
                normalizedTitle,
                command.Description,
                command.RequesterId,
                command.Priority,
                timeProvider);

            await ticketRepository.AddAsync(ticket, cancellationToken);
            await unitOfWork.CommitAsync(cancellationToken);

            return new CreateTicketResult(ticket.Id, ticket.Title, ticket.Status, ticket.OpenedAtUtc);
        }
    }
}

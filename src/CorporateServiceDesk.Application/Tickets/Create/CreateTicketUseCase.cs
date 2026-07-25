using CorporateServiceDesk.Application.Common.Abstractions;
using CorporateServiceDesk.Application.Common.Abstractions.Notifications;
using CorporateServiceDesk.Application.Common.Abstractions.Persistence;
using CorporateServiceDesk.Application.Tickets.Abstractions;
using CorporateServiceDesk.Domain.Tickets.Entities;


namespace CorporateServiceDesk.Application.Tickets.Create
{
    public sealed class CreateTicketUseCase(ITicketRepository ticketRepository, IUnitOfWork unitOfWork, TimeProvider timeProvider) : IUseCase
    {
        public async Task<Result<CreateTicketResult>> ExecuteAsync(CreateTicketCommand command, CancellationToken cancellationToken)
        {
            var normalizedTitle = command.Title.Trim();
            var duplicateExists = await ticketRepository.ExistsByTitleForRequesterAsync(command.RequesterId, normalizedTitle, cancellationToken);

            if (duplicateExists)
            {
                return Result<CreateTicketResult>.Failure("A similar open ticket already exists for this requester.", EnumErrorType.Conflict);
            }

            var ticket = Ticket.Open(
                            normalizedTitle,
                            command.Description,
                            command.RequesterId,
                            command.Priority,
                            timeProvider);

            await ticketRepository.AddAsync(ticket, cancellationToken);

            await unitOfWork.CommitAsync(cancellationToken);

            
            return Result<CreateTicketResult>.Success(new CreateTicketResult(
                                                                    ticket.Id,
                                                                    ticket.Title,
                                                                    ticket.Description.Trim(),
                                                                    ticket.RequesterId,
                                                                    ticket.Priority,
                                                                    ticket.Status,
                                                                    ticket.OpenedAtUtc),
                                                       EnumErrorType.Created);
            
        }
    }
}




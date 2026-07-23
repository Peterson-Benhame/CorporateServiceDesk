using CorporateServiceDesk.Application.Common.Abstractions.Persistence;
using CorporateServiceDesk.Domain.Tickets.Entities;

namespace CorporateServiceDesk.Application.Tickets.Abstractions
{
    public interface ITicketRepository : IRepositoryBase<Ticket>
    {
        Task<Ticket?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<bool> ExistsByTitleForRequesterAsync(Guid requesterId, string title, CancellationToken cancellationToken);
    }

}

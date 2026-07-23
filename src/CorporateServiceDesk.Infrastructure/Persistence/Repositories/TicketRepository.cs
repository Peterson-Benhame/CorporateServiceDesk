using CorporateServiceDesk.Application.Tickets.Abstractions;
using CorporateServiceDesk.Domain.Tickets.Entities;
using CorporateServiceDesk.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace CorporateServiceDesk.Infrastructure.Persistence.Repositories
{
    public sealed class TicketRepository(ApplicationDbContext dbContext)
    : RepositoryBase<Ticket>(dbContext), ITicketRepository
    {
        public Task<Ticket?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
            => DbSet.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        public Task<bool> ExistsByTitleForRequesterAsync(Guid requesterId, string title, CancellationToken cancellationToken)
            => DbSet.AnyAsync(x => x.RequesterId == requesterId && x.Title == title, cancellationToken);
    }
}

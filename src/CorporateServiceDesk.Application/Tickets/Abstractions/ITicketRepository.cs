using CorporateServiceDesk.Application.Common.Abstractions.Persistence;
using CorporateServiceDesk.Application.Common.Pagination;
using CorporateServiceDesk.Application.Common.Specifications;
using CorporateServiceDesk.Application.Tickets.Queries.List;
using CorporateServiceDesk.Domain.Tickets.Entities;

namespace CorporateServiceDesk.Application.Tickets.Abstractions;

public interface ITicketRepository : IRepositoryBase<Ticket>
{
    Task<Ticket?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> ExistsByTitleForRequesterAsync(
        Guid requesterId,
        string title,
        CancellationToken cancellationToken);
    Task<PagedResult<QueryTicketListItemResult>> QueryAsync(
        ISpecification<Ticket> specification,
        PageRequest pagination,
        SortRequest<TicketSortColumn>? sort,
        CancellationToken cancellationToken);
}

using CorporateServiceDesk.Application.Common.Pagination;
using CorporateServiceDesk.Application.Common.Specifications;
using CorporateServiceDesk.Application.Tickets.Abstractions;
using CorporateServiceDesk.Application.Tickets.Queries.List;
using CorporateServiceDesk.Domain.Tickets.Entities;
using CorporateServiceDesk.Domain.Tickets.Enums;
using CorporateServiceDesk.Infrastructure.Pagination;
using CorporateServiceDesk.Infrastructure.Persistence.Contexts;
using CorporateServiceDesk.Infrastructure.Specifications;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CorporateServiceDesk.Infrastructure.Persistence.Repositories;

public sealed class TicketRepository(ApplicationDbContext dbContext)
    : RepositoryBase<Ticket>(dbContext), ITicketRepository
{
    private static readonly Expression<Func<Ticket, QueryTicketListItemResult>>
        ListProjection = ticket => new(
            ticket.Id, ticket.Title, ticket.RequesterId, ticket.AssigneeId,
            ticket.Priority, ticket.Status, ticket.OpenedAtUtc, ticket.ClosedAtUtc);

    public Task<Ticket?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        DbSet.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<bool> ExistsByTitleForRequesterAsync(
        Guid requesterId, string title, CancellationToken cancellationToken)
    {
        var activeStatuses = new[]
        {
            TicketStatus.Open, TicketStatus.InProgress, TicketStatus.Waiting
        };
        return DbSet.AnyAsync(ticket =>
            ticket.RequesterId == requesterId &&
            ticket.Title == title &&
            activeStatuses.Contains(ticket.Status), cancellationToken);
    }

    public Task<PagedResult<QueryTicketListItemResult>> QueryAsync(
        ISpecification<Ticket> specification,
        PageRequest pagination,
        SortRequest<TicketSortColumn>? sort,
        CancellationToken cancellationToken)
    {
        var query = SpecificationEvaluator.Apply(DbSet.AsNoTracking(), specification);
        var ordered = ApplyOrdering(query, sort);
        return ordered.ToPagedResultAsync(pagination, ListProjection, cancellationToken);
    }

    private static IOrderedQueryable<Ticket> ApplyOrdering(
        IQueryable<Ticket> query,
        SortRequest<TicketSortColumn>? sort)
    {
        var column = sort?.Column ?? TicketSortColumn.OpenedAtUtc;
        var direction = sort?.Direction ?? SortDirection.Descending;

        IOrderedQueryable<Ticket> ordered = (column, direction) switch
        {
            (TicketSortColumn.Title, SortDirection.Ascending) => query.OrderBy(x => x.Title),
            (TicketSortColumn.Title, _) => query.OrderByDescending(x => x.Title),
            (TicketSortColumn.Status, SortDirection.Ascending) => query.OrderBy(x => x.Status),
            (TicketSortColumn.Status, _) => query.OrderByDescending(x => x.Status),
            (TicketSortColumn.Priority, SortDirection.Ascending) => query.OrderBy(x => x.Priority),
            (TicketSortColumn.Priority, _) => query.OrderByDescending(x => x.Priority),
            (TicketSortColumn.ClosedAtUtc, SortDirection.Ascending) => query.OrderBy(x => x.ClosedAtUtc),
            (TicketSortColumn.ClosedAtUtc, _) => query.OrderByDescending(x => x.ClosedAtUtc),
            (TicketSortColumn.OpenedAtUtc, SortDirection.Ascending) => query.OrderBy(x => x.OpenedAtUtc),
            _ => query.OrderByDescending(x => x.OpenedAtUtc)
        };

        return ordered.ThenBy(x => x.Id);
    }
}

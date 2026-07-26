using CorporateServiceDesk.Application.Common.Specifications;
using CorporateServiceDesk.Domain.Tickets.Entities;

namespace CorporateServiceDesk.Application.Tickets.Queries.List.Specifications;

public sealed class TicketListSpecification : Specification<Ticket>
{
    public TicketListSpecification(QueryListTicketsFilter filter)
    {
        if (filter.Status.HasValue)
            AddAnd(ticket => ticket.Status == filter.Status);
        if (filter.Priority.HasValue)
            AddAnd(ticket => ticket.Priority == filter.Priority);
        if (filter.RequesterId.HasValue)
            AddAnd(ticket => ticket.RequesterId == filter.RequesterId);
        if (filter.AssigneeId.HasValue)
            AddAnd(ticket => ticket.AssigneeId == filter.AssigneeId);
        if (filter.OpenedFromUtc.HasValue)
            AddAnd(ticket => ticket.OpenedAtUtc >= filter.OpenedFromUtc);
        if (filter.OpenedToUtc.HasValue)
            AddAnd(ticket => ticket.OpenedAtUtc <= filter.OpenedToUtc);
        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var search = filter.Search.Trim();
            AddAnd(ticket => ticket.Title.Contains(search));
        }
    }
}

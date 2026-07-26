using CorporateServiceDesk.Application.Common.Pagination;

namespace CorporateServiceDesk.Application.Tickets.Queries.List;

public sealed record QuerySearchTicketsFilter(
    PageRequest Pagination,
    SortRequest<TicketSortColumn>? Sort,
    IReadOnlyList<TicketFilterCriterion> Criteria);

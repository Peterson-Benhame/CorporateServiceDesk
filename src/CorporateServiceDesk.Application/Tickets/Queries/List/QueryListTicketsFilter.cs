using CorporateServiceDesk.Application.Common.Pagination;
using CorporateServiceDesk.Domain.Tickets.Enums;

namespace CorporateServiceDesk.Application.Tickets.Queries.List;

public sealed record QueryListTicketsFilter(
    PageRequest Pagination,
    SortRequest<TicketSortColumn>? Sort = null,
    TicketStatus? Status = null,
    TicketPriority? Priority = null,
    Guid? RequesterId = null,
    Guid? AssigneeId = null,
    DateTimeOffset? OpenedFromUtc = null,
    DateTimeOffset? OpenedToUtc = null,
    string? Search = null);

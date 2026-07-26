namespace CorporateServiceDesk.Application.Common.Pagination;

public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    int Count,
    int? TotalCount,
    int? TotalPages);

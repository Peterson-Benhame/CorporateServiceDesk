using CorporateServiceDesk.Application.Common.Pagination;

namespace CorporateServiceDesk.Api.Contracts.Common;

public sealed record PagedResponse<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    int Count,
    int? TotalCount,
    int? TotalPages);

internal static class PagedResponseMapper
{
    public static PagedResponse<TResponse> Map<TSource, TResponse>(
        PagedResult<TSource> result,
        Func<TSource, TResponse> mapper) =>
        new(
            result.Items.Select(mapper).ToList(),
            result.Page,
            result.PageSize,
            result.Count,
            result.TotalCount,
            result.TotalPages);
}

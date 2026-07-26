using CorporateServiceDesk.Application.Common.Pagination;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using ApplicationPagedResult = CorporateServiceDesk.Application.Common.Pagination;

namespace CorporateServiceDesk.Infrastructure.Pagination;

public static class QueryablePaginationExtensions
{
    public static async Task<ApplicationPagedResult.PagedResult<TResult>>
        ToPagedResultAsync<TSource, TResult>(
        this IQueryable<TSource> source,
        IPagination pagination,
        Expression<Func<TSource, TResult>> projection,
        CancellationToken cancellationToken)
    {
        var totalCount = pagination.CountTotal
            ? await source.CountAsync(cancellationToken)
            : (int?)null;

        var items = await source
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .Select(projection)
            .ToListAsync(cancellationToken);

        var totalPages = totalCount.HasValue
            ? (int)Math.Ceiling(totalCount.Value / (double)pagination.PageSize)
            : (int?)null;

        return new ApplicationPagedResult.PagedResult<TResult>(
            items,
            pagination.Page,
            pagination.PageSize,
            items.Count,
            totalCount,
            totalPages);
    }
}

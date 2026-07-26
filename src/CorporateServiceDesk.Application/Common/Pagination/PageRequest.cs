namespace CorporateServiceDesk.Application.Common.Pagination;

public sealed record PageRequest(
    int Page = 1,
    int PageSize = 10,
    bool CountTotal = false) : IPagination
{
    public const int DefaultPage = 1;
    public const int DefaultPageSize = 10;
    public const int MaximumPageSize = 100;

    public PageRequest Normalize() =>
        this with
        {
            Page = Page < DefaultPage ? DefaultPage : Page,
            PageSize = PageSize < 1
                ? DefaultPageSize
                : Math.Min(PageSize, MaximumPageSize)
        };
}

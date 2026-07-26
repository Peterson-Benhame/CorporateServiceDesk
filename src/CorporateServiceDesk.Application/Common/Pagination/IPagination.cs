namespace CorporateServiceDesk.Application.Common.Pagination;

public interface IPagination
{
    int Page { get; }
    int PageSize { get; }
    bool CountTotal { get; }
}

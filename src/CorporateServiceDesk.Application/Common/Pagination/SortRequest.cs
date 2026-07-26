namespace CorporateServiceDesk.Application.Common.Pagination;

public sealed record SortRequest<TColumn>(
    TColumn Column,
    SortDirection Direction = SortDirection.Descending)
    where TColumn : struct, Enum;

namespace CorporateServiceDesk.Infrastructure.Pagination.Interfaces.Inputs
{
    public interface IPagination : IOrderable
    {
        int ItemsPerPage { get; set; }
        int Page { get; set; }
        bool CountTotal { get; set; }
    }
}
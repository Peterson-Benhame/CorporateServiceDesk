namespace CorporateServiceDesk.Infrastructure.Pagination.Filters
{
    public class ObjectFilter : DynamicFilter
    {
        public int? FilterObjectTypeId { get; set; }
        public string FiltroName { get; set; }
    }
}

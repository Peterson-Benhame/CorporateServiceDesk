
using CorporateServiceDesk.Infrastructure.Pagination.OperationResult;

namespace CorporateServiceDesk.Infrastructure.Pagination.Interfaces.Inputs
{
    public interface IOrderable
    {
        OrdenationAttribute Ordenations { get; set; }
    }
}
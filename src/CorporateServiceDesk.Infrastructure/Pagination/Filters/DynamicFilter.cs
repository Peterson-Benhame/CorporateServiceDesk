using CorporateServiceDesk.Infrastructure.Pagination.Enums;
using CorporateServiceDesk.Infrastructure.Pagination.OperationResult;

namespace CorporateServiceDesk.Infrastructure.Pagination.Filters
{
   
    public class DynamicFilter : BaseFilter
    {
        public EnumFiltroPorColuna FiltroColuna { get; set; }

        public EnumFiltroOperador FiltroOperador { get; set; }

        public bool? FiltroValor { get; set; }
    }
}
using System.ComponentModel;

namespace CorporateServiceDesk.Infrastructure.Pagination.Enums
{
    public enum EnumFiltroOperador
    {
        [Description("Active")]
        Active = 1,

        [Description("Disabled")]
        Disabled = 2,

        [Description("All")]
        All = 3
    }
}

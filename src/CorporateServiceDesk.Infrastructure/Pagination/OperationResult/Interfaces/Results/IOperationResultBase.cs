
using CorporateServiceDesk.Infrastructure.Pagination.Enums.Utils;

namespace CorporateServiceDesk.Infrastructure.Pagination.Interfaces.Results
{
    public interface IOperationResultBase
    {
        List<string> Messages { get; set; }
        EnumResultType ResultType { get; set; }
        Exception Exception { get; set; }
        bool IsSuccessResultType { get; }

        IOperationResultBase AddMessage(string message);

        IOperationResultBase AddMessages(IEnumerable<string> messages);
    }
}
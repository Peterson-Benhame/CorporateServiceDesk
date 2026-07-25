using System.Diagnostics.CodeAnalysis;
using CorporateServiceDesk.Infrastructure.Pagination.Enums.Utils;
using CorporateServiceDesk.Infrastructure.Pagination.Interfaces.Results;

namespace CorporateServiceDesk.Infrastructure.Pagination.OperationResult
{

    [ExcludeFromCodeCoverageAttribute]
    public class OperationResultBase : IOperationResultBase
    {
        public List<string> Messages { get; set; }
        public EnumResultType ResultType { get; set; }
        public Exception Exception { get; set; }

#pragma warning disable CA1822
        public bool IsSuccessResultType => _IsSuccessResultType(ResultType);
#pragma warning restore CA1822

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected OperationResultBase(EnumResultType resultType)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            ResultType = resultType;
        }

        protected OperationResultBase(IOperationResultBase otherResult)
        {
            Messages = otherResult.Messages;
            ResultType = otherResult.ResultType;
            Exception = otherResult.Exception;
        }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected OperationResultBase(EnumResultType resultType, Exception exception)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            ResultType = resultType;
            Exception = exception;
        }

        public static IOperationResultBase Create(EnumResultType resultType)
            => new OperationResultBase(resultType);

        public static IOperationResultBase Create(IOperationResultBase otherResult)
            => new OperationResultBase(otherResult);

        public static IOperationResultBase Success()
            => new OperationResultBase(EnumResultType.Ok);

        public static IOperationResultBase Created()
            => new OperationResultBase(EnumResultType.Created);

        public static IOperationResultBase InvalidInput()
            => new OperationResultBase(EnumResultType.InvalidInput);

        public static IOperationResultBase ServiceUnavailable()
            => new OperationResultBase(EnumResultType.ServiceUnavailable);

        public static IOperationResultBase NotFound()
            => new OperationResultBase(EnumResultType.NotFound);

        public static IOperationResultBase InternalError()
            => new OperationResultBase(EnumResultType.InternalServerError);

        public static IOperationResultBase InternalError(Exception exception)
            => new OperationResultBase(EnumResultType.InternalServerError) { Exception = exception };

        public IOperationResultBase AddMessage(string message)
        {
            _AddMessage(message);
            return this;
        }

        public IOperationResultBase AddMessages(IEnumerable<string> messages)
        {
            _AddMessages(messages);
            return this;
        }

        private static bool _IsSuccessResultType(EnumResultType resultType)
        {
            switch (resultType)
            {
                case EnumResultType.Ok:
                case EnumResultType.Created:
                case EnumResultType.Accepted:
                case EnumResultType.NoContent: return true;
                default: return false;
            }
        }

        protected void _AddMessage(string message)
        {
            Messages ??= new List<string>();
            Messages.Add(message);
        }

        protected void _AddMessages(IEnumerable<string> messages)
        {
            if (messages is null)
                return;

            Messages ??= new List<string>();
            Messages.AddRange(messages);
        }
    }
}
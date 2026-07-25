using CorporateServiceDesk.Infrastructure.Pagination.Enums.Utils;
using CorporateServiceDesk.Infrastructure.Pagination.Interfaces.Results;
using System.Diagnostics.CodeAnalysis;

namespace CorporateServiceDesk.Infrastructure.Pagination.OperationResult
{

    [ExcludeFromCodeCoverageAttribute]
    public class OperationResult<T> : OperationResultBase, IOperationResult<T>
    {
        public T Data { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected OperationResult(EnumResultType resultType) : base(resultType)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
        }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected OperationResult(IOperationResultBase otherResult) : base(otherResult)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
        }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected OperationResult(EnumResultType resultType, Exception exception) : base(resultType, exception)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
        }

        protected OperationResult(EnumResultType resultType, T result) : base(resultType)
        {
            Data = result;
        }

        public static new IOperationResult<T> Create(IOperationResultBase otherResult)
            => new OperationResult<T>(otherResult);

        public static new IOperationResult<T> Create(EnumResultType resultType)
            => new OperationResult<T>(resultType);

        public static IOperationResult<T> Create(EnumResultType resultType, T data)
            => new OperationResult<T>(resultType, data);

        public static IOperationResult<T> Success(T data)
            => new OperationResult<T>(EnumResultType.Ok, data);

        public static IOperationResult<T> Created(T data)
            => new OperationResult<T>(EnumResultType.Created, data);

        public new static IOperationResult<T> InvalidInput()
            => new OperationResult<T>(EnumResultType.InvalidInput);

        public new static IOperationResult<T> ServiceUnavailable()
            => new OperationResult<T>(EnumResultType.ServiceUnavailable);

        public new static IOperationResult<T> NotFound()
            => new OperationResult<T>(EnumResultType.NotFound);

        public new static IOperationResult<T> InternalError()
            => new OperationResult<T>(EnumResultType.InternalServerError);

        public new static IOperationResult<T> InternalError(Exception exception)
            => new OperationResult<T>(EnumResultType.InternalServerError) { Exception = exception };

        public new IOperationResult<T> AddMessage(string message)
        {
            _AddMessage(message);
            return this;
        }

        public new IOperationResult<T> AddMessages(IEnumerable<string> messages)
        {
            _AddMessages(messages);
            return this;
        }
    }
}
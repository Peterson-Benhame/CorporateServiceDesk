using CorporateServiceDesk.Application.Tickets.Create;
using System.Net;

namespace CorporateServiceDesk.Application.Common.Abstractions.Notifications
{
    public sealed class Result<T>
    {
        public bool IsSuccess { get; }
        public string? Error { get; }
        public EnumErrorType? ErrorType { get; }
        public T? Value { get; }

        private Result(T value, EnumErrorType resultType)
        {
            IsSuccess = true;
            Value = value;
            ErrorType = resultType;
        }

        private Result(string error, EnumErrorType errorType)
        {
            IsSuccess = false;
            Error = error;
            ErrorType = errorType;
        }

        public static Result<T> Success(
            T value,
            EnumErrorType resultType = EnumErrorType.OK)
        {
            if (resultType is not EnumErrorType.OK and
                not EnumErrorType.Created and
                not EnumErrorType.NoContent)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(resultType),
                    resultType,
                    "A successful result must use a success status.");
            }

            return new Result<T>(value, resultType);
        }

        public static Result<T> Failure(string error, EnumErrorType errorType)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(error);

            return new Result<T>(error, errorType);
        }

        public Result<TOutput> Map<TOutput>(Func<T, TOutput> mapper)
        {
            ArgumentNullException.ThrowIfNull(mapper);

            if (!IsSuccess)
            {
                return Result<TOutput>.Failure(Error ?? "Não foi possível concluir a operação.", ErrorType ?? EnumErrorType.Validation);
            }

            return Result<TOutput>.Success(
                mapper(Value!),
                ErrorType ?? EnumErrorType.OK);
        }
    }
    public enum EnumErrorType
    {
        OK,
        Created,
        Validation,
        NotFound,
        NoContent,
        Conflict,
        BadRequest,
        InternalServerError
    }
}

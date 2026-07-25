using CorporateServiceDesk.Infrastructure.Pagination.Enums.Utils;
using CorporateServiceDesk.Infrastructure.Pagination.Interfaces.Inputs;
using CorporateServiceDesk.Infrastructure.Pagination.Interfaces.Results;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Linq.Dynamic.Core;

namespace CorporateServiceDesk.Infrastructure.Pagination.OperationResult
{
    [ExcludeFromCodeCoverageAttribute]
    public class PaginatedResult : OperationResult<IList>, IPaginatedResult
    {
        public int? TotalCount { get; set; }
        public int? Pages { get; set; }
        public int Count { get; set; }
        public int Page { get; set; }

        protected PaginatedResult(EnumResultType resultType) : base(resultType)
        {
        }

        protected PaginatedResult(IOperationResultBase otherResult) : base(otherResult)
        {
        }

        protected PaginatedResult(EnumResultType resultType, Exception exception) : base(resultType, exception)
        {
        }

        public new static IPaginatedResult Create(IOperationResultBase otherResult)
            => new PaginatedResult(otherResult);

        public new static IPaginatedResult Create(EnumResultType resultType)
            => new PaginatedResult(resultType);

        public static IPaginatedResult Success<TSource, TResult>(IPagination pagination, IQueryable<TSource> source, Func<TSource, TResult> selectFunction)
        {
            var result = new PaginatedResult(EnumResultType.Ok);
            result.Paginate(pagination, source, selectFunction);

            return result;
        }

        public static IPaginatedResult Success<TSource>(IPagination pagination, IQueryable<TSource> source)
        {
            var result = new PaginatedResult(EnumResultType.Ok);
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            result.Paginate<TSource, TSource>(pagination, source, null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            return result;
        }

        public new static IPaginatedResult InvalidInput()
            => new PaginatedResult(EnumResultType.InvalidInput);

        public new static IPaginatedResult ServiceUnavailable()
            => new PaginatedResult(EnumResultType.ServiceUnavailable);

        public new static IPaginatedResult NotFound()
            => new PaginatedResult(EnumResultType.NotFound);

        public new static IPaginatedResult InternalError()
            => new PaginatedResult(EnumResultType.InternalServerError);

        public new static IPaginatedResult InternalError(Exception exception)
            => new PaginatedResult(EnumResultType.InternalServerError) { Exception = exception };

        public void Paginate<TSource, TResult>(IPagination pagination, IQueryable<TSource> source, Func<TSource, TResult> selectFunction)
        {
            Page = pagination.Page == 0 ? 1 : pagination.Page;

            var orderedSource = Order(source, pagination);
            var paginatedSource = orderedSource.Skip(Page * pagination.ItemsPerPage - pagination.ItemsPerPage).Take(pagination.ItemsPerPage);

            if (pagination.CountTotal)
            {
                TotalCount = source.Count();

                double pages = TotalCount.Value / (double)pagination.ItemsPerPage;

                Pages = (int)(pages % 1 == 0 ? pages : pages + 1);
            }

            if (selectFunction != null)
                Data = paginatedSource.Select(selectFunction).ToList();
            else
                Data = paginatedSource.ToList();

            Count = Data.Count;
        }

        private static IQueryable<TSource> Order<TSource>(IQueryable<TSource> source, IOrderable ordenator)
        {
            if (ordenator.Ordenations is null || string.IsNullOrEmpty(ordenator.Ordenations.PropertyName))
                return source;

            var query = new StringBuilder();

            query.Append(ordenator.Ordenations.PropertyName);
            query.Append(ordenator.Ordenations.Direction == ListSortDirection.Descending ? " desc" : " asc");
            query.Append(',');

            query = query.Remove(query.Length - 1, 1);
            return source.OrderBy(query.ToString());
        }

        IPaginatedResult IPaginatedResult.AddMessage(string message)
        {
            _AddMessage(message);
            return this;
        }
    }
}
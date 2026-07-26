using CorporateServiceDesk.Application.Common.Abstractions;
using CorporateServiceDesk.Application.Common.Abstractions.Notifications;
using CorporateServiceDesk.Application.Common.Pagination;
using CorporateServiceDesk.Application.Tickets.Abstractions;
using CorporateServiceDesk.Application.Tickets.Queries.List.Specifications;
using FluentValidation;

namespace CorporateServiceDesk.Application.Tickets.Queries.List;

public sealed class QueryListTicketsUseCase(
    ITicketRepository repository,
    IValidator<QueryListTicketsFilter> validator) : IUseCase
{
    public async Task<Result<PagedResult<QueryTicketListItemResult>>> ExecuteAsync(
        QueryListTicketsFilter filter,
        CancellationToken cancellationToken)
    {
        var normalized = filter with { Pagination = filter.Pagination.Normalize() };
        var validation = await validator.ValidateAsync(normalized, cancellationToken);
        if (!validation.IsValid)
            return Result<PagedResult<QueryTicketListItemResult>>.Failure(
                string.Join("; ", validation.Errors.Select(error => error.ErrorMessage)),
                EnumErrorType.BadRequest);

        var page = await repository.QueryAsync(
            new TicketListSpecification(normalized),
            normalized.Pagination,
            normalized.Sort,
            cancellationToken);
        return Result<PagedResult<QueryTicketListItemResult>>.Success(page);
    }
}

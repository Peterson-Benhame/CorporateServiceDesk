using CorporateServiceDesk.Application.Common.Validation;
using FluentValidation;

namespace CorporateServiceDesk.Application.Tickets.Queries.List.Validation;

public sealed class QueryListTicketsFilterValidator : AbstractValidator<QueryListTicketsFilter>
{
    public QueryListTicketsFilterValidator()
    {
        RuleFor(x => x.Pagination).SetValidator(new PageRequestValidator());
        RuleFor(x => x).Must(x =>
                !x.OpenedFromUtc.HasValue || !x.OpenedToUtc.HasValue ||
                x.OpenedFromUtc <= x.OpenedToUtc)
            .WithMessage("OpenedFromUtc cannot be later than OpenedToUtc.");
        RuleFor(x => x.Sort!.Column).IsInEnum().When(x => x.Sort is not null);
        RuleFor(x => x.Sort!.Direction).IsInEnum().When(x => x.Sort is not null);
    }
}

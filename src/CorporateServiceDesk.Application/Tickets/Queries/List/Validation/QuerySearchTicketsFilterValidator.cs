using CorporateServiceDesk.Application.Common.Validation;
using FluentValidation;

namespace CorporateServiceDesk.Application.Tickets.Queries.List.Validation;

public sealed class QuerySearchTicketsFilterValidator : AbstractValidator<QuerySearchTicketsFilter>
{
    public QuerySearchTicketsFilterValidator()
    {
        RuleFor(x => x.Pagination).SetValidator(new PageRequestValidator());
        RuleFor(x => x.Criteria).NotNull();
        RuleForEach(x => x.Criteria).SetValidator(new TicketFilterCriterionValidator());
        RuleFor(x => x.Sort!.Column).IsInEnum().When(x => x.Sort is not null);
        RuleFor(x => x.Sort!.Direction).IsInEnum().When(x => x.Sort is not null);
    }
}

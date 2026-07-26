using CorporateServiceDesk.Application.Common.Pagination;
using FluentValidation;

namespace CorporateServiceDesk.Application.Common.Validation;

public sealed class PageRequestValidator : AbstractValidator<PageRequest>
{
    public PageRequestValidator()
    {
        RuleFor(page => page.PageSize).LessThanOrEqualTo(PageRequest.MaximumPageSize);
    }
}

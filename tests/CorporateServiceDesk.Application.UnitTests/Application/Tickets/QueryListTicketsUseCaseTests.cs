using CorporateServiceDesk.Application.Common.Pagination;
using CorporateServiceDesk.Application.Common.Specifications;
using CorporateServiceDesk.Application.Tickets.Abstractions;
using CorporateServiceDesk.Application.Tickets.Queries.List;
using CorporateServiceDesk.Application.Tickets.Queries.List.Specifications;
using CorporateServiceDesk.Application.Tickets.Queries.List.Validation;
using CorporateServiceDesk.Domain.Tickets.Entities;
using FluentValidation.TestHelper;
using Moq;

namespace CorporateServiceDesk.Application.UnitTests.Tickets;

public sealed class QueryListTicketsUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldNormalizeAndPropagateCancellation()
    {
        var repository = new Mock<ITicketRepository>();
        var token = new CancellationTokenSource().Token;
        var expected = new PagedResult<QueryTicketListItemResult>(
            [], 1, 100, 0, null, null);
        repository.Setup(x => x.QueryAsync(
                It.IsAny<ISpecification<Ticket>>(),
                It.Is<PageRequest>(p => p.Page == 1 && p.PageSize == 100),
                null, token))
            .ReturnsAsync(expected);

        var useCase = new QueryListTicketsUseCase(
            repository.Object, new QueryListTicketsFilterValidator());
        var result = await useCase.ExecuteAsync(
            new QueryListTicketsFilter(new PageRequest(0, 200)), token);

        Assert.True(result.IsSuccess);
        Assert.Same(expected, result.Value);
        repository.VerifyAll();
    }

    [Fact]
    public async Task ExecuteAsync_ShouldRejectInvalidDateRange()
    {
        var repository = new Mock<ITicketRepository>();
        var useCase = new QueryListTicketsUseCase(
            repository.Object, new QueryListTicketsFilterValidator());
        var filter = new QueryListTicketsFilter(
            new PageRequest(),
            OpenedFromUtc: DateTimeOffset.UtcNow,
            OpenedToUtc: DateTimeOffset.UtcNow.AddDays(-1));

        var result = await useCase.ExecuteAsync(filter, CancellationToken.None);

        Assert.False(result.IsSuccess);
        repository.Verify(x => x.QueryAsync(
            It.IsAny<ISpecification<Ticket>>(), It.IsAny<PageRequest>(),
            It.IsAny<SortRequest<TicketSortColumn>?>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}

public sealed class TicketFilterCriterionValidatorTests
{
    [Fact]
    public void Validator_ShouldRejectContainsForStatus()
    {
        var validator = new TicketFilterCriterionValidator();
        var result = validator.TestValidate(new TicketFilterCriterion(
            TicketFilterColumn.Status, FilterOperator.Contains, "Open"));
        result.ShouldHaveValidationErrorFor(x => x.Operator);
    }

    [Fact]
    public void Specification_ShouldGroupOrBeforeAnd()
    {
        var tickets = new[]
        {
            Create("VPN Open"),
            Create("Printer")
        };
        var specification = new TicketSearchSpecification([
            new(TicketFilterColumn.Status, FilterOperator.Equals, "Open"),
            new(TicketFilterColumn.Title, FilterOperator.Contains, "VPN", LogicalOperator.And)
        ]);

        var result = tickets.AsQueryable().Where(specification.Criteria!).ToList();
        Assert.Single(result);
        Assert.Equal("VPN Open", result[0].Title);
    }

    private static Ticket Create(string title) => Ticket.Open(
        title, "Sufficiently detailed description.", Guid.NewGuid(),
        CorporateServiceDesk.Domain.Tickets.Enums.TicketPriority.Low,
        TimeProvider.System);
}


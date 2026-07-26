using CorporateServiceDesk.Application.Common.Abstractions.Notifications;
using CorporateServiceDesk.Application.Tickets.Abstractions;
using CorporateServiceDesk.Application.Tickets.Queries;
using CorporateServiceDesk.Domain.Tickets.Entities;
using CorporateServiceDesk.Domain.Tickets.Enums;
using Moq;

namespace CorporateServiceDesk.Application.UnitTests.Application.Tickets;

public sealed class QueryGetTicketByIdUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ShouldReturnTicket_WhenItExists()
    {
        var repository = new Mock<ITicketRepository>();
        var ticket = Ticket.Open(
            "VPN unavailable",
            "Cannot connect to the corporate VPN.",
            Guid.NewGuid(),
            TicketPriority.High,
            TimeProvider.System);

        repository
            .Setup(x => x.GetByIdAsync(ticket.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);

        var useCase = new QueryGetTicketByIdUseCase(repository.Object);

        var result = await useCase.ExecuteAsync(ticket.Id, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(EnumErrorType.OK, result.ErrorType);
        Assert.Equal(ticket.Id, result.Value.Id);
        Assert.Equal(ticket.Title, result.Value.Title);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnNotFound_WhenTicketDoesNotExist()
    {
        var repository = new Mock<ITicketRepository>();
        var ticketId = Guid.NewGuid();
        var useCase = new QueryGetTicketByIdUseCase(repository.Object);

        var result = await useCase.ExecuteAsync(ticketId, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Null(result.Value);
        Assert.Equal(EnumErrorType.NotFound, result.ErrorType);
        Assert.Equal("Ticket was not found.", result.Error);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldPassCancellationTokenToRepository()
    {
        var repository = new Mock<ITicketRepository>();
        var cancellationToken = new CancellationTokenSource().Token;
        var ticketId = Guid.NewGuid();
        var useCase = new QueryGetTicketByIdUseCase(repository.Object);

        await useCase.ExecuteAsync(ticketId, cancellationToken);

        repository.Verify(
            x => x.GetByIdAsync(ticketId, cancellationToken),
            Times.Once);
    }
}


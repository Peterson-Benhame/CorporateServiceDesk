using CorporateServiceDesk.Domain.Common.Common;
using CorporateServiceDesk.Domain.Tickets.Entities;
using CorporateServiceDesk.Domain.Tickets.Enums;
using System;

namespace CorporateServiceDesk.UnitTests.Domain.Tickets;

public sealed class TicketCloseTests
{
    [Fact]
    public void Close_ShouldThrowDomainException_WhenStatusIsNotResolved()
    {
        var requesterId = Guid.NewGuid();
        var timeProvider = TimeProvider.System;

        var openTicket = Ticket.Open("Title", "Description", requesterId, TicketPriority.Low, timeProvider);
        Assert.Throws<DomainException>(() => openTicket.Close(timeProvider));

        var inProgressTicket = Ticket.Open("Title", "Description", requesterId, TicketPriority.Low, timeProvider);
        inProgressTicket.AssignTo(Guid.NewGuid());
        Assert.Throws<DomainException>(() => inProgressTicket.Close(timeProvider));
    }

    [Fact]
    public void Close_ShouldSetStatusToClosedAndSetClosedAtUtc_WhenStatusIsResolved()
    {
        var requesterId = Guid.NewGuid();
        var timeProvider = TimeProvider.System;
        var expectedClosedAt = timeProvider.GetUtcNow();

        var ticket = Ticket.Open("Title", "Description", requesterId, TicketPriority.High, timeProvider);
        ticket.AssignTo(Guid.NewGuid());
        ticket.Resolve();

        ticket.Close(timeProvider);

        Assert.Equal(TicketStatus.Closed, ticket.Status);
        Assert.True(ticket.ClosedAtUtc.HasValue);
        Assert.Equal(expectedClosedAt.UtcDateTime.Date, ticket.ClosedAtUtc.Value.UtcDateTime.Date);
    }
}
using CorporateServiceDesk.Domain.Common.Common;
using CorporateServiceDesk.Domain.Tickets.Entities;
using CorporateServiceDesk.Domain.Tickets.Enums;
using System;

namespace CorporateServiceDesk.UnitTests.Domain.Tickets;

public sealed class TicketResolveTests
{
    [Fact]
    public void Resolve_ShouldThrowDomainException_WhenStatusIsNotInProgress()
    {
        var requesterId = Guid.NewGuid();
        var timeProvider = TimeProvider.System;
        var ticket = Ticket.Open("Title", "Description", requesterId, TicketPriority.Low, timeProvider);

        var action = () => ticket.Resolve();

        Assert.Throws<DomainException>(action);
    }

    [Fact]
    public void Resolve_ShouldSetStatusToResolved_WhenInProgress()
    {
        var requesterId = Guid.NewGuid();
        var timeProvider = TimeProvider.System;
        var ticket = Ticket.Open("Title", "Description", requesterId, TicketPriority.High, timeProvider);

        ticket.AssignTo(Guid.NewGuid()); // InProgress
        ticket.Resolve();

        Assert.Equal(TicketStatus.Resolved, ticket.Status);
    }
}
using CorporateServiceDesk.Domain.Common.Common;
using CorporateServiceDesk.Domain.Tickets.Entities;
using CorporateServiceDesk.Domain.Tickets.Enums;
using System;

namespace CorporateServiceDesk.UnitTests.Domain.Tickets;

public sealed class TicketAssignTests
{
    [Fact]
    public void AssignTo_ShouldThrowDomainException_WhenAssigneeIdIsEmpty()
    {
        var requesterId = Guid.NewGuid();
        var timeProvider = TimeProvider.System;
        var ticket = Ticket.Open("Title", "Description", requesterId, TicketPriority.Low, timeProvider);

        var action = () => ticket.AssignTo(Guid.Empty);

        Assert.Throws<DomainException>(action);
    }

    [Fact]
    public void AssignTo_ShouldThrowDomainException_WhenTicketIsClosed()
    {
        var requesterId = Guid.NewGuid();
        var timeProvider = TimeProvider.System;
        var ticket = Ticket.Open("Title", "Description", requesterId, TicketPriority.Low, timeProvider);

        ticket.AssignTo(Guid.NewGuid()); // InProgress
        ticket.Resolve(); // Resolved
        ticket.Close(timeProvider); // Closed

        var action = () => ticket.AssignTo(Guid.NewGuid());

        Assert.Throws<DomainException>(action);
    }

    [Fact]
    public void AssignTo_ShouldAssignAndSetStatusToInProgress_WhenTicketIsOpen()
    {
        var requesterId = Guid.NewGuid();
        var timeProvider = TimeProvider.System;
        var ticket = Ticket.Open("Title", "Description", requesterId, TicketPriority.Medium, timeProvider);

        var assignee = Guid.NewGuid();
        ticket.AssignTo(assignee);

        Assert.Equal(assignee, ticket.AssigneeId);
        Assert.Equal(TicketStatus.InProgress, ticket.Status);
    }

    [Fact]
    public void AssignTo_ShouldReassign_WhenTicketIsInProgress()
    {
        var requesterId = Guid.NewGuid();
        var timeProvider = TimeProvider.System;
        var ticket = Ticket.Open("Title", "Description", requesterId, TicketPriority.Medium, timeProvider);

        var first = Guid.NewGuid();
        ticket.AssignTo(first); // now InProgress

        var second = Guid.NewGuid();
        ticket.AssignTo(second);

        Assert.Equal(second, ticket.AssigneeId);
        Assert.Equal(TicketStatus.InProgress, ticket.Status);
    }
}
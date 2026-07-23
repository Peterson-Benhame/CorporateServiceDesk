using CorporateServiceDesk.Domain.Tickets.Entities;
using CorporateServiceDesk.Domain.Tickets.Enums;

namespace CorporateServiceDesk.UnitTests.Domain.Tickets;

public sealed class TicketPriorityValidationTests
{
    [Fact]
    public void Create_ShouldAcceptAllDefinedPriorities()
    {
        var requesterId = Guid.NewGuid();
        var timeProvider = TimeProvider.System;

        foreach (var value in Enum.GetValues(typeof(TicketPriority)))
        {
            var priority = (TicketPriority)value;
            var ticket = Ticket.Open("Title", "Description", requesterId, priority, timeProvider);

            Assert.Equal(priority, ticket.Priority);
        }
    }
}
using CorporateServiceDesk.Domain.Common.Common;
using CorporateServiceDesk.Domain.Tickets.Entities;
using CorporateServiceDesk.Domain.Tickets.Enums;

namespace CorporateServiceDesk.UnitTests.Domain.Tickets;

public sealed class TicketTests
{
    [Fact]
    public void Create_ShouldCreateOpenTicket_WhenDataIsValid()
    {
        // Arrange
        var requesterId = Guid.NewGuid();
        var timeProvider = TimeProvider.System;
        var openedAtUtc = timeProvider.GetUtcNow();

        // Act
        var ticket = Ticket.Open(
            "Unable to access corporate system",
            "The system returns an access denied message.",
            requesterId,
            TicketPriority.High,
            timeProvider);

        // Assert
        Assert.NotEqual(Guid.Empty, ticket.Id);
        Assert.Equal("Unable to access corporate system", ticket.Title);
        Assert.Equal("The system returns an access denied message.", ticket.Description);
        Assert.Equal(requesterId, ticket.RequesterId);
        Assert.Equal(TicketPriority.High, ticket.Priority);
        Assert.Equal(TicketStatus.Open, ticket.Status);
        Assert.Equal(openedAtUtc.UtcDateTime.Date, ticket.OpenedAtUtc.UtcDateTime.Date);
    }

    [Fact]
    public void Create_ShouldTrimTitleAndDescription()
    {
        // Arrange
        var requesterId = Guid.NewGuid();
        var timeProvider = TimeProvider.System;

        // Act
        var ticket = Ticket.Open(
            "  Access problem  ",
            "  User cannot access the system.  ",
            requesterId,
            TicketPriority.Medium,
            timeProvider);

        // Assert
        Assert.Equal("Access problem", ticket.Title);
        Assert.Equal(
            "User cannot access the system.",
            ticket.Description);
    }

    [Fact]
    public void Create_ShouldThrowDomainException_WhenTitleIsEmpty()
    {
        // Arrange
        var requesterId = Guid.NewGuid();
        var timeProvider = TimeProvider.System;

        // Act
        var action = () => Ticket.Open(
            "",
            "Valid description",
            requesterId,
            TicketPriority.Low,
            timeProvider);

        // Assert
        Assert.Throws<DomainException>(action);
    }

    [Fact]
    public void Create_ShouldThrowDomainException_WhenDescriptionIsEmpty()
    {
        // Arrange
        var requesterId = Guid.NewGuid();
        var timeProvider = TimeProvider.System;

        // Act
        var action = () => Ticket.Open(
            "Valid title",
            "   ",
            requesterId,
            TicketPriority.Low,
            timeProvider);

        // Assert
        Assert.Throws<DomainException>(action);
    }

    [Fact]
    public void Create_ShouldThrowDomainException_WhenRequesterIdIsEmpty()
    {
        // Arrange
        var timeProvider = TimeProvider.System;

        // Act
        var action = () => Ticket.Open(
            "Valid title",
            "Valid description",
            Guid.Empty,
            TicketPriority.Low,
            timeProvider);

        // Assert
        Assert.Throws<DomainException>(action);
    }

    [Fact]
    public void Create_ShouldThrowDomainException_WhenPriorityIsInvalid()
    {
        // Arrange
        var invalidPriority = (TicketPriority)999;
        var timeProvider = TimeProvider.System;

        // Act
        var action = () => Ticket.Open(
            "Valid title",
            "Valid description",
            Guid.NewGuid(),
            invalidPriority,
            timeProvider);

        // Assert
        Assert.Throws<DomainException>(action);
    }

    [Fact]
    public void Open_ShouldThrowDomainException_WhenTitleExceedsMaxLength()
    {
        var requesterId = Guid.NewGuid();
        var timeProvider = TimeProvider.System;
        var longTitle = new string('A', 161); // limite é 160

        var action = () => Ticket.Open(longTitle, "Valid description", requesterId, TicketPriority.Low, timeProvider);

        Assert.Throws<DomainException>(action);
    }
}
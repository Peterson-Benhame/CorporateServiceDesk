using CorporateServiceDesk.Domain.Tickets.Entities;
using CorporateServiceDesk.Domain.Tickets.Enums;
using CorporateServiceDesk.Domain.Tickets.Exceptions;

namespace CorporateServiceDesk.UnitTests.Domain.Tickets;

public sealed class TicketTests
{
    [Fact]
    public void Create_ShouldCreateOpenTicket_WhenDataIsValid()
    {
        // Arrange
        var requesterId = Guid.NewGuid();
        var createdAt = new DateTimeOffset(
            2026,
            7,
            16,
            20,
            0,
            0,
            TimeSpan.Zero);

        // Act
        var ticket = Ticket.Create(
            "Unable to access corporate system",
            "The system returns an access denied message.",
            requesterId,
            TicketPriority.High,
            createdAt);

        // Assert
        Assert.NotEqual(Guid.Empty, ticket.Id);
        Assert.Equal("Unable to access corporate system", ticket.Title);
        Assert.Equal(
            "The system returns an access denied message.",
            ticket.Description);
        Assert.Equal(requesterId, ticket.RequesterId);
        Assert.Equal(TicketPriority.High, ticket.Priority);
        Assert.Equal(TicketStatus.Open, ticket.Status);
        Assert.Equal(createdAt, ticket.CreatedAt);
    }

    [Fact]
    public void Create_ShouldTrimTitleAndDescription()
    {
        // Arrange
        var requesterId = Guid.NewGuid();
        var createdAt = DateTimeOffset.UtcNow;

        // Act
        var ticket = Ticket.Create(
            "  Access problem  ",
            "  User cannot access the system.  ",
            requesterId,
            TicketPriority.Medium,
            createdAt);

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

        // Act
        var action = () => Ticket.Create(
            "   ",
            "Valid description",
            requesterId,
            TicketPriority.Low,
            DateTimeOffset.UtcNow);

        // Assert
        Assert.Throws<DomainException>(action);
    }

    [Fact]
    public void Create_ShouldThrowDomainException_WhenDescriptionIsEmpty()
    {
        // Arrange
        var requesterId = Guid.NewGuid();

        // Act
        var action = () => Ticket.Create(
            "Valid title",
            "   ",
            requesterId,
            TicketPriority.Low,
            DateTimeOffset.UtcNow);

        // Assert
        Assert.Throws<DomainException>(action);
    }

    [Fact]
    public void Create_ShouldThrowDomainException_WhenRequesterIdIsEmpty()
    {
        // Act
        var action = () => Ticket.Create(
            "Valid title",
            "Valid description",
            Guid.Empty,
            TicketPriority.Low,
            DateTimeOffset.UtcNow);

        // Assert
        Assert.Throws<DomainException>(action);
    }

    [Fact]
    public void Create_ShouldThrowDomainException_WhenPriorityIsInvalid()
    {
        // Arrange
        var invalidPriority = (TicketPriority)999;

        // Act
        var action = () => Ticket.Create(
            "Valid title",
            "Valid description",
            Guid.NewGuid(),
            invalidPriority,
            DateTimeOffset.UtcNow);

        // Assert
        Assert.Throws<DomainException>(action);
    }

    [Fact]
    public void Create_ShouldThrowDomainException_WhenCreatedAtIsNotUtc()
    {
        // Arrange
        var nonUtcDate = new DateTimeOffset(
            2026,
            7,
            16,
            20,
            0,
            0,
            TimeSpan.FromHours(-3));

        // Act
        var action = () => Ticket.Create(
            "Valid title",
            "Valid description",
            Guid.NewGuid(),
            TicketPriority.Low,
            nonUtcDate);

        // Assert
        Assert.Throws<DomainException>(action);
    }
}
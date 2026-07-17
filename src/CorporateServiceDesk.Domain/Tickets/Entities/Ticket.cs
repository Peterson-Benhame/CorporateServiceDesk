using CorporateServiceDesk.Domain.Tickets.Enums;
using CorporateServiceDesk.Domain.Tickets.Exceptions;

namespace CorporateServiceDesk.Domain.Tickets.Entities;

public sealed class Ticket
{
    private Ticket()
    {
        Title = string.Empty;
        Description = string.Empty;
    }

    private Ticket(
        Guid id,
        string title,
        string description,
        Guid requesterId,
        TicketPriority priority,
        TicketStatus status,
        DateTimeOffset createdAt)
    {
        Id = id;
        Title = title;
        Description = description;
        RequesterId = requesterId;
        Priority = priority;
        Status = status;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }

    public string Title { get; private set; }

    public string Description { get; private set; }

    public Guid RequesterId { get; private set; }

    public TicketPriority Priority { get; private set; }

    public TicketStatus Status { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public static Ticket Create(
        string title,
        string description,
        Guid requesterId,
        TicketPriority priority,
        DateTimeOffset createdAt)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new DomainException("Ticket title is required.");
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new DomainException("Ticket description is required.");
        }

        if (requesterId == Guid.Empty)
        {
            throw new DomainException("Ticket requester is required.");
        }

        if (!Enum.IsDefined(typeof(TicketPriority), priority))
        {
            throw new DomainException("Ticket priority is invalid.");
        }

        if (createdAt.Offset != TimeSpan.Zero)
        {
            throw new DomainException("Ticket creation date must be in UTC.");
        }

        return new Ticket(
            Guid.NewGuid(),
            title.Trim(),
            description.Trim(),
            requesterId,
            priority,
            TicketStatus.Open,
            createdAt);
    }
}
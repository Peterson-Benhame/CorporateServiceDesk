using CorporateServiceDesk.Domain.Common.Common;
using CorporateServiceDesk.Domain.Tickets.Enums;

namespace CorporateServiceDesk.Domain.Tickets.Entities;

public sealed class Ticket
{
    private Ticket() { } // Necessario apenas para materializacao do EF Core.
    private Ticket(Guid id, string title, string description, Guid requesterId, TicketPriority priority, DateTimeOffset openedAtUtc)
    {
        if (id == Guid.Empty) throw new DomainException("Ticket id is required.");
        if (requesterId == Guid.Empty) throw new DomainException("Requester is required.");
        if (string.IsNullOrWhiteSpace(title)) throw new DomainException("Title is required.");
        if (title.Trim().Length > 160) throw new DomainException("Title cannot exceed 160 characters.");
        if (string.IsNullOrWhiteSpace(description)) throw new DomainException("Description isrequired.");
        Id = id;
        Title = title.Trim();
        Description = description.Trim();
        RequesterId = requesterId;
        Priority = priority;
        Status = TicketStatus.Open;
        OpenedAtUtc = openedAtUtc;
    }

    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public Guid RequesterId { get; private set; }
    public Guid? AssigneeId { get; private set; }
    public TicketPriority Priority { get; private set; }
    public TicketStatus Status { get; private set; }
    public DateTimeOffset OpenedAtUtc { get; private set; }
    public DateTimeOffset? ClosedAtUtc { get; private set; }

    public static Ticket Open(string title, string description, Guid requesterId, TicketPriority priority, TimeProvider timeProvider)
    {
        if (!Enum.IsDefined(typeof(TicketPriority), priority))
            throw new DomainException("Priority is invalid.");

        return new(Guid.NewGuid(), title, description, requesterId, priority, timeProvider.GetUtcNow());
    }

    public void AssignTo(Guid assigneeId)
    {
        if (assigneeId == Guid.Empty) 
            throw new DomainException("Assignee is required.");

        if (Status == TicketStatus.Closed) 
            throw new DomainException("Closed tickets cannot be assigned.");

        AssigneeId = assigneeId;

        if (Status == TicketStatus.Open) 
            Status = TicketStatus.InProgress;
    }

    public void Resolve()
    {
        if (Status != TicketStatus.InProgress) 
            throw new DomainException("Only in-progress tickets can be resolved.");

        Status = TicketStatus.Resolved;
    }

    public void Close(TimeProvider timeProvider)
    {
        if (Status != TicketStatus.Resolved) 
            throw new DomainException("Only resolved tickets can be closed.");

        Status = TicketStatus.Closed;
        ClosedAtUtc = timeProvider.GetUtcNow();
    }
}
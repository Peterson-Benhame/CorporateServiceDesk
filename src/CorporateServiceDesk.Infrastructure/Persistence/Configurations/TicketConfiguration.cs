using CorporateServiceDesk.Domain.Tickets.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CorporateServiceDesk.Infrastructure.Persistence.Configurations.Tickets;

public sealed class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.ToTable("tickets");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(x => x.Title)
            .HasColumnName("title")
            .HasMaxLength(160)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasColumnName("description")
            .HasMaxLength(4000)
            .IsRequired();

        builder.Property(x => x.RequesterId)
            .HasColumnName("requester_id")
            .IsRequired();

        builder.Property(x => x.AssigneeId)
            .HasColumnName("assignee_id");

        builder.Property(x => x.Priority)
            .HasColumnName("priority").
            HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(x => x.Status)
            .HasColumnName("status").HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(x => x.OpenedAtUtc)
            .HasColumnName("opened_at_utc")
            .IsRequired();

        builder.Property(x => x.ClosedAtUtc)
            .HasColumnName("closed_at_utc");

        builder.HasIndex(x => new { x.Status, x.OpenedAtUtc });
        builder.HasIndex(x => x.RequesterId);
        builder.HasIndex(x => x.AssigneeId);
    }

}
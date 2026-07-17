using CorporateServiceDesk.Domain.Tickets.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CorporateServiceDesk.Infrastructure.Persistence.Configurations.Tickets;

public sealed class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.ToTable("tickets");

        builder.HasKey(ticket => ticket.Id);

        builder.Property(ticket => ticket.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(ticket => ticket.Title)
            .HasColumnName("title")
            .IsRequired();

        builder.Property(ticket => ticket.Description)
            .HasColumnName("description")
            .IsRequired();

        builder.Property(ticket => ticket.RequesterId)
            .HasColumnName("requester_id")
            .IsRequired();

        builder.Property(ticket => ticket.Priority)
            .HasColumnName("priority")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(ticket => ticket.Status)
            .HasColumnName("status")
            .HasConversion<int>()
            .IsRequired();

        builder.Property(ticket => ticket.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone")
            .IsRequired();
    }
}
using CinemaTicket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CinemaTicket.Persistence.Configurations;

public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.ToTable("Tickets");

        builder.HasKey(t => t.Id);

        // Configure properties
        builder.Property(t => t.Price)
               .IsRequired()
               .HasPrecision(18, 2);

        builder.Property(t => t.TicketNumber)
               .IsRequired()
               .HasMaxLength(50);

        builder.Property(t => t.HolderName)
               .IsRequired()
               .HasMaxLength(100);

        builder.Property(t => t.Status)
               .IsRequired()
               .HasConversion<string>()
               .HasMaxLength(50);

        builder.Property(t => t.CreatedAt)
               .IsRequired();

        // Configure relationships with explicit delete behaviors
        builder.HasOne(t => t.Showtime)
               .WithMany(s => s.Tickets)
               .HasForeignKey(t => t.ShowtimeId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.User)
               .WithMany(u => u.Tickets)
               .HasForeignKey(t => t.UserId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Seat)
               .WithMany(s => s.Tickets)
               .HasForeignKey(t => t.SeatId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Payment)
               .WithOne(p => p.Ticket)
               .HasForeignKey<Payment>(p => p.TicketId)
               .OnDelete(DeleteBehavior.Cascade);

        // Configure indexes
        builder.HasIndex(t => t.TicketNumber)
               .IsUnique();

        builder.HasIndex(t => new { t.ShowtimeId, t.SeatId })
               .IsUnique();
    }
}

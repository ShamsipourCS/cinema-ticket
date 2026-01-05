using CinemaTicket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Price)
               .HasPrecision(18, 2);

        builder.Property(t => t.TicketNumber)
               .HasMaxLength(50);

        builder.Property(t => t.HolderName)
               .HasMaxLength(100);

        builder.HasOne(t => t.Showtime)
               .WithMany(s => s.Tickets)
               .HasForeignKey(t => t.ShowtimeId);

        builder.HasOne(t => t.User)
               .WithMany()
               .HasForeignKey(t => t.UserId);

        builder.HasOne(t => t.Seat)
               .WithMany()
               .HasForeignKey(t => t.SeatId);

        builder.HasOne(t => t.Payment)
               .WithOne(p => p.Ticket)
               .HasForeignKey<Payment>(p => p.TicketId);

        builder.HasIndex(t => new { t.ShowtimeId, t.SeatId })
               .IsUnique();
    }
}

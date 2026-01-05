using CinemaTicket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CinemaTicket.Persistence.Configurations
{
    public class SeatConfiguration : IEntityTypeConfiguration<Seat>
    {
        public void Configure(EntityTypeBuilder<Seat> builder)
        {
            builder.ToTable("Seats");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Row)
                .IsRequired()
                .HasMaxLength(5);

            builder.Property(s => s.Number)
                .IsRequired();

            builder.Property(s => s.SeatType)
                .IsRequired();

            builder.Property(s => s.PriceMultiplier)
                .IsRequired()
                .HasPrecision(5, 2);

            builder.HasOne(s => s.Hall)
                .WithMany(h => h.Seats)
                .HasForeignKey(s => s.HallId)
                .OnDelete(DeleteBehavior.Cascade);

            // Prevent duplicate seat positions in a hall
            builder.HasIndex(s => new { s.HallId, s.Row, s.Number })
                .IsUnique();
        }
    }
}

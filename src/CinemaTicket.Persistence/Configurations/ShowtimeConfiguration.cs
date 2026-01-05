using CinemaTicket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


public class ShowtimeConfiguration : IEntityTypeConfiguration<Showtime>
{
    public void Configure(EntityTypeBuilder<Showtime> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.BasePrice)
               .HasPrecision(18, 2);

        builder.HasOne(s => s.Movie)
               .WithMany()
               .HasForeignKey(s => s.MovieId);

        builder.HasOne(s => s.Hall)
               .WithMany()
               .HasForeignKey(s => s.HallId);
    }
}

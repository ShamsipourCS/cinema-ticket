namespace CinemaTicket.Persistence.Configurations
{
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
                   .WithMany(m => m.Showtimes)
                   .HasForeignKey(s => s.MovieId)
                   .OnDelete(DeleteBehavior.Restrict); 

            builder.HasOne(s => s.Hall)
                   .WithMany(h => h.Showtimes)
                   .HasForeignKey(s => s.HallId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(s => s.StartTime);

            builder.HasCheckConstraint("CK_Showtime_EndAfterStart", "[EndTime] > [StartTime]");

        }
    }
}

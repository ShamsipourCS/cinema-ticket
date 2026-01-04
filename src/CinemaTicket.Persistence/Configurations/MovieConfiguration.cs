using CinemaTicket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CinemaTicket.Persistence.Configurations
{
    public class MovieConfiguration : IEntityTypeConfiguration<Movie>
    {
        public void Configure(EntityTypeBuilder<Movie> builder)
        {
            // Table name
            builder.ToTable("Movies");

            // Primary key
            builder.HasKey(m => m.Id);


            // Properties
            builder.Property(m => m.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(m => m.Description)
                .HasMaxLength(2000);

            builder.Property(m => m.DurationMinutes)
                .IsRequired();

            builder.Property(m => m.Genre)
                .HasMaxLength(100);

            builder.Property(m => m.Rating)
                .HasMaxLength(10);

            builder.Property(m => m.PosterUrl)
                .HasMaxLength(500);

            builder.Property(m => m.ReleaseDate)
                .IsRequired();

            builder.Property(m => m.IsActive)
                .IsRequired();

            // Relationships
            builder.HasMany(m => m.Showtimes)
                .WithOne(s => s.Movie)
                .HasForeignKey(s => s.MovieId);
        }
    }
}

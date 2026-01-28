using CinemaTicket.Domain.Entities;
using CinemaTicket.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace CinemaTicket.Persistence.Seeders;

/// <summary>
/// Seeds movie catalog with diverse, realistic movie data.
/// </summary>
public static class MovieSeeder
{
    /// <summary>
    /// Seeds movies. This method is idempotent.
    /// </summary>
    public static async Task SeedAsync(
        ApplicationDbContext context,
        CancellationToken cancellationToken = default)
    {
        // Check if movies already exist (idempotency)
        if (await context.Movies.AnyAsync(cancellationToken))
            return;

        var movies = new List<Movie>
        {
            new Movie
            {
                Id = new Guid("10000001-0001-0001-0001-000000000001"),
                Title = "Interstellar",
                Description = "A team of explorers travel through a wormhole in space in an attempt to ensure humanity's survival.",
                Genre = "Sci-Fi",
                DurationMinutes = 169,
                Rating = "PG-13",
                ReleaseDate = new DateTime(2014, 11, 7),
                PosterUrl = "https://via.placeholder.com/300x450?text=Interstellar",
                IsActive = true
            },
            new Movie
            {
                Id = new Guid("10000002-0001-0001-0001-000000000002"),
                Title = "The Matrix",
                Description = "A computer hacker learns from mysterious rebels about the true nature of his reality and his role in the war against its controllers.",
                Genre = "Action",
                DurationMinutes = 136,
                Rating = "R",
                ReleaseDate = new DateTime(1999, 3, 31),
                PosterUrl = "https://via.placeholder.com/300x450?text=The+Matrix",
                IsActive = true
            },
            new Movie
            {
                Id = new Guid("10000003-0001-0001-0001-000000000003"),
                Title = "Inception",
                Description = "A thief who steals corporate secrets through the use of dream-sharing technology is given the inverse task of planting an idea into the mind of a C.E.O.",
                Genre = "Sci-Fi",
                DurationMinutes = 148,
                Rating = "PG-13",
                ReleaseDate = new DateTime(2010, 7, 16),
                PosterUrl = "https://via.placeholder.com/300x450?text=Inception",
                IsActive = true
            },
            new Movie
            {
                Id = new Guid("10000004-0001-0001-0001-000000000004"),
                Title = "The Shawshank Redemption",
                Description = "Two imprisoned men bond over a number of years, finding solace and eventual redemption through acts of common decency.",
                Genre = "Drama",
                DurationMinutes = 142,
                Rating = "R",
                ReleaseDate = new DateTime(1994, 9, 23),
                PosterUrl = "https://via.placeholder.com/300x450?text=Shawshank+Redemption",
                IsActive = true
            },
            new Movie
            {
                Id = new Guid("10000005-0001-0001-0001-000000000005"),
                Title = "The Dark Knight",
                Description = "When the menace known as the Joker wreaks havoc and chaos on the people of Gotham, Batman must accept one of the greatest psychological and physical tests of his ability to fight injustice.",
                Genre = "Action",
                DurationMinutes = 152,
                Rating = "PG-13",
                ReleaseDate = new DateTime(2008, 7, 18),
                PosterUrl = "https://via.placeholder.com/300x450?text=The+Dark+Knight",
                IsActive = true
            },
            new Movie
            {
                Id = new Guid("10000006-0001-0001-0001-000000000006"),
                Title = "Pulp Fiction",
                Description = "The lives of two mob hitmen, a boxer, a gangster and his wife intertwine in four tales of violence and redemption.",
                Genre = "Crime",
                DurationMinutes = 154,
                Rating = "R",
                ReleaseDate = new DateTime(1994, 10, 14),
                PosterUrl = "https://via.placeholder.com/300x450?text=Pulp+Fiction",
                IsActive = true
            },
            new Movie
            {
                Id = new Guid("10000007-0001-0001-0001-000000000007"),
                Title = "Forrest Gump",
                Description = "The presidencies of Kennedy and Johnson, the Vietnam War, and other historical events unfold from the perspective of an Alabama man with an IQ of 75.",
                Genre = "Drama",
                DurationMinutes = 142,
                Rating = "PG-13",
                ReleaseDate = new DateTime(1994, 7, 6),
                PosterUrl = "https://via.placeholder.com/300x450?text=Forrest+Gump",
                IsActive = true
            },
            new Movie
            {
                Id = new Guid("10000008-0001-0001-0001-000000000008"),
                Title = "The Godfather",
                Description = "The aging patriarch of an organized crime dynasty transfers control of his clandestine empire to his reluctant son.",
                Genre = "Crime",
                DurationMinutes = 175,
                Rating = "R",
                ReleaseDate = new DateTime(1972, 3, 24),
                PosterUrl = "https://via.placeholder.com/300x450?text=The+Godfather",
                IsActive = true
            },
            new Movie
            {
                Id = new Guid("10000009-0001-0001-0001-000000000009"),
                Title = "Avengers: Endgame",
                Description = "After the devastating events of Infinity War, the Avengers assemble once more to reverse Thanos' actions and restore balance to the universe.",
                Genre = "Action",
                DurationMinutes = 181,
                Rating = "PG-13",
                ReleaseDate = new DateTime(2019, 4, 26),
                PosterUrl = "https://via.placeholder.com/300x450?text=Avengers+Endgame",
                IsActive = true
            },
            new Movie
            {
                Id = new Guid("1000000A-0001-0001-0001-00000000000A"),
                Title = "Parasite",
                Description = "Greed and class discrimination threaten the newly formed symbiotic relationship between the wealthy Park family and the destitute Kim clan.",
                Genre = "Thriller",
                DurationMinutes = 132,
                Rating = "R",
                ReleaseDate = new DateTime(2019, 5, 30),
                PosterUrl = "https://via.placeholder.com/300x450?text=Parasite",
                IsActive = true
            }
        };

        await context.Movies.AddRangeAsync(movies, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}

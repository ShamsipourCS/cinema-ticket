using CinemaTicket.Domain.Entities;
using CinemaTicket.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace CinemaTicket.Persistence.Seeders;

public static class CatalogSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context, CancellationToken cancellationToken = default)
    {
        await context.Database.MigrateAsync(cancellationToken);

        if (!await context.Cinemas.AnyAsync(cancellationToken))
        {
            var cinema1Id = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var cinema2Id = Guid.Parse("22222222-2222-2222-2222-222222222222");

            var cinema1 = new Cinema
            {
                Id = cinema1Id,
                Name = "Cinema One",
                Address = "123 Main St",
                City = "Tehran",
                Phone = "021-111111",
                IsActive = true
            };

            var cinema2 = new Cinema
            {
                Id = cinema2Id,
                Name = "Cinema Two",
                Address = "456 Elm St",
                City = "Tehran",
                Phone = "021-222222",
                IsActive = true
            };

            await context.Cinemas.AddRangeAsync(new[] { cinema1, cinema2 }, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            // Now seed Halls that reference the real cinema IDs above
            var hall1 = new Hall
            {
                Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                CinemaId = cinema1Id,
                Name = "Hall A",
                Rows = 10,
                SeatsPerRow = 15,
                TotalCapacity = 150
            };

            var hall2 = new Hall
            {
                Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                CinemaId = cinema1Id,
                Name = "Hall B",
                Rows = 12,
                SeatsPerRow = 20,
                TotalCapacity = 240
            };

            var hall3 = new Hall
            {
                Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                CinemaId = cinema2Id,
                Name = "Hall C",
                Rows = 8,
                SeatsPerRow = 12,
                TotalCapacity = 96
            };

            await context.Halls.AddRangeAsync(new[] { hall1, hall2, hall3 }, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        // Seed Movies (independent)
        if (!await context.Movies.AnyAsync(cancellationToken))
        {
            var movie1 = new Movie
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Title = "Interstellar",
                Description = "A team travels through a wormhole in space.",
                DurationMinutes = 169,
                Genre = "Sci-Fi",
                Rating = "PG-13",
                PosterUrl = "https://example.com/interstellar.jpg",
                ReleaseDate = new DateTime(2014, 11, 7),
                IsActive = true
            };

            var movie2 = new Movie
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Title = "The Matrix",
                Description = "A hacker discovers the nature of his reality.",
                DurationMinutes = 136,
                Genre = "Action",
                Rating = "R",
                PosterUrl = "https://example.com/matrix.jpg",
                ReleaseDate = new DateTime(1999, 3, 31),
                IsActive = true
            };

            await context.Movies.AddRangeAsync(new[] { movie1, movie2 }, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}

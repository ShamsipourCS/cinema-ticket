using CinemaTicket.Domain.Entities;
using CinemaTicket.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace CinemaTicket.Persistence.Seeders;

/// <summary>
/// Seeds cinema infrastructure including cinemas and halls.
/// Halls automatically generate seats based on Rows × SeatsPerRow configuration.
/// </summary>
public static class CinemaSeeder
{
    /// <summary>
    /// Seeds cinemas and halls. This method is idempotent.
    /// </summary>
    public static async Task SeedAsync(
        ApplicationDbContext context,
        CancellationToken cancellationToken = default)
    {
        // Check if cinemas already exist (idempotency)
        if (await context.Cinemas.AnyAsync(cancellationToken))
            return;

        // Create cinemas first
        var cinemas = new List<Cinema>
        {
            new Cinema
            {
                Id = new Guid("CINEMA01-0001-0001-0001-000000000001"),
                Name = "Cinema One - Downtown",
                Address = "123 Main Street",
                City = "Tehran",
                Phone = "+98-21-1111-1111",
                IsActive = true
            },
            new Cinema
            {
                Id = new Guid("CINEMA02-0001-0001-0001-000000000002"),
                Name = "Cinema Two - Westside",
                Address = "456 Elm Street",
                City = "Tehran",
                Phone = "+98-21-2222-2222",
                IsActive = true
            },
            new Cinema
            {
                Id = new Guid("CINEMA03-0001-0001-0001-000000000003"),
                Name = "Cinema Three - Eastside",
                Address = "789 Oak Avenue",
                City = "Tehran",
                Phone = "+98-21-3333-3333",
                IsActive = true
            }
        };

        await context.Cinemas.AddRangeAsync(cinemas, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        // Create halls for each cinema (seats will be auto-generated)
        var halls = new List<Hall>
        {
            // Cinema One - Downtown (2 halls)
            new Hall
            {
                Id = new Guid("HALL0001-0001-0001-0001-000000000001"),
                CinemaId = new Guid("CINEMA01-0001-0001-0001-000000000001"),
                Name = "Hall A",
                Rows = 10,
                SeatsPerRow = 15,
                TotalCapacity = 150
            },
            new Hall
            {
                Id = new Guid("HALL0002-0001-0001-0001-000000000002"),
                CinemaId = new Guid("CINEMA01-0001-0001-0001-000000000001"),
                Name = "Hall B",
                Rows = 12,
                SeatsPerRow = 20,
                TotalCapacity = 240
            },
            new Hall
            {
                Id = new Guid("HALL0003-0001-0001-0001-000000000003"),
                CinemaId = new Guid("CINEMA01-0001-0001-0001-000000000001"),
                Name = "Hall C",
                Rows = 15,
                SeatsPerRow = 20,
                TotalCapacity = 300
            },

            // Cinema Two - Westside (3 halls)
            new Hall
            {
                Id = new Guid("HALL0004-0001-0001-0001-000000000004"),
                CinemaId = new Guid("CINEMA02-0001-0001-0001-000000000002"),
                Name = "Hall D",
                Rows = 8,
                SeatsPerRow = 12,
                TotalCapacity = 96
            },
            new Hall
            {
                Id = new Guid("HALL0005-0001-0001-0001-000000000005"),
                CinemaId = new Guid("CINEMA02-0001-0001-0001-000000000002"),
                Name = "Hall E",
                Rows = 10,
                SeatsPerRow = 16,
                TotalCapacity = 160
            },
            new Hall
            {
                Id = new Guid("HALL0006-0001-0001-0001-000000000006"),
                CinemaId = new Guid("CINEMA02-0001-0001-0001-000000000002"),
                Name = "Hall F",
                Rows = 12,
                SeatsPerRow = 18,
                TotalCapacity = 216
            },

            // Cinema Three - Eastside (3 halls)
            new Hall
            {
                Id = new Guid("HALL0007-0001-0001-0001-000000000007"),
                CinemaId = new Guid("CINEMA03-0001-0001-0001-000000000003"),
                Name = "Hall G",
                Rows = 10,
                SeatsPerRow = 18,
                TotalCapacity = 180
            },
            new Hall
            {
                Id = new Guid("HALL0008-0001-0001-0001-000000000008"),
                CinemaId = new Guid("CINEMA03-0001-0001-0001-000000000003"),
                Name = "Hall H",
                Rows = 14,
                SeatsPerRow = 20,
                TotalCapacity = 280
            },
            new Hall
            {
                Id = new Guid("HALL0009-0001-0001-0001-000000000009"),
                CinemaId = new Guid("CINEMA03-0001-0001-0001-000000000003"),
                Name = "Hall I",
                Rows = 16,
                SeatsPerRow = 22,
                TotalCapacity = 352
            }
        };

        await context.Halls.AddRangeAsync(halls, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}

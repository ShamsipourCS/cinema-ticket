using CinemaTicket.Domain.Entities;
using CinemaTicket.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace CinemaTicket.Persistence.Seeders;

/// <summary>
/// Seeds showtimes linking movies to halls over the next 7 days.
/// Implements realistic scheduling with time-based pricing.
/// </summary>
public static class ShowtimeSeeder
{
    /// <summary>
    /// Seeds showtimes. This method is idempotent.
    /// </summary>
    public static async Task SeedAsync(
        ApplicationDbContext context,
        CancellationToken cancellationToken = default)
    {
        // Check if showtimes already exist (idempotency)
        if (await context.Showtimes.AnyAsync(cancellationToken))
            return;

        // Get all movies and halls to create showtimes
        var movies = await context.Movies.ToListAsync(cancellationToken);
        var halls = await context.Halls.ToListAsync(cancellationToken);

        if (!movies.Any() || !halls.Any())
            return; // Cannot seed showtimes without movies and halls

        var showtimes = new List<Showtime>();
        var showtimeIdCounter = 1;

        // Time slots for showtimes (in hours)
        var timeSlots = new[] { 10, 13, 16, 19, 22 }; // 10:00 AM, 1:00 PM, 4:00 PM, 7:00 PM, 10:00 PM

        // Generate showtimes for next 7 days
        var startDate = DateTime.Today;
        for (int day = 0; day < 7; day++)
        {
            var currentDate = startDate.AddDays(day);

            // Distribute movies across halls for each day
            foreach (var hall in halls)
            {
                // Assign 2-3 different movies per hall per day
                var moviesForHall = GetRandomMovies(movies, 2 + (day % 2)); // Alternate between 2 and 3 movies

                foreach (var timeSlot in timeSlots.Take(moviesForHall.Count))
                {
                    var movie = moviesForHall[timeSlots.ToList().IndexOf(timeSlot) % moviesForHall.Count];
                    var startTime = currentDate.AddHours(timeSlot);

                    // Calculate end time: movie duration + 15 minutes buffer
                    var endTime = startTime.AddMinutes(movie.DurationMinutes + 15);

                    // Check if this time slot would overlap with existing showtimes in this hall
                    var wouldOverlap = showtimes.Any(s =>
                        s.HallId == hall.Id &&
                        s.StartTime.Date == currentDate &&
                        ((startTime >= s.StartTime && startTime < s.EndTime) ||
                         (endTime > s.StartTime && endTime <= s.EndTime) ||
                         (startTime <= s.StartTime && endTime >= s.EndTime)));

                    if (wouldOverlap)
                        continue; // Skip this showtime to avoid overlap

                    // Calculate base price based on time of day
                    var basePrice = CalculateBasePrice(timeSlot);

                    var showtime = new Showtime
                    {
                        Id = new Guid($"SHOWTIME{showtimeIdCounter:D4}-0001-0001-0001-000000000001"),
                        MovieId = movie.Id,
                        HallId = hall.Id,
                        StartTime = startTime,
                        EndTime = endTime,
                        BasePrice = basePrice,
                        IsActive = true
                    };

                    showtimes.Add(showtime);
                    showtimeIdCounter++;
                }
            }
        }

        await context.Showtimes.AddRangeAsync(showtimes, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Gets a random subset of movies.
    /// </summary>
    private static List<Movie> GetRandomMovies(List<Movie> allMovies, int count)
    {
        var random = new Random(42); // Fixed seed for deterministic results
        return allMovies.OrderBy(_ => random.Next()).Take(count).ToList();
    }

    /// <summary>
    /// Calculates base price based on time of day.
    /// Morning: $8, Matinee: $10, Evening: $15, Late night: $12
    /// </summary>
    private static decimal CalculateBasePrice(int hour)
    {
        return hour switch
        {
            < 12 => 8.00m,      // Morning shows (before noon)
            < 17 => 10.00m,     // Matinee shows (noon to 5 PM)
            < 22 => 15.00m,     // Evening shows (5 PM to 10 PM)
            _ => 12.00m         // Late night shows (after 10 PM)
        };
    }
}

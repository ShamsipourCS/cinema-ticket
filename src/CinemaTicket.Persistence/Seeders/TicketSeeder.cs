using CinemaTicket.Domain.Entities;
using CinemaTicket.Domain.Enums;
using CinemaTicket.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace CinemaTicket.Persistence.Seeders;

/// <summary>
/// Seeds sample ticket bookings for testing purposes.
/// This is optional and can be disabled for production environments.
/// </summary>
public static class TicketSeeder
{
    /// <summary>
    /// Seeds sample tickets. This method is idempotent.
    /// </summary>
    public static async Task SeedAsync(
        ApplicationDbContext context,
        CancellationToken cancellationToken = default)
    {
        // Check if tickets already exist (idempotency)
        if (await context.Tickets.AnyAsync(cancellationToken))
            return;

        // Get required entities for creating tickets
        var users = await context.Users.Where(u => u.Role == UserRole.Customer).ToListAsync(cancellationToken);
        var showtimes = await context.Showtimes
            .Include(s => s.Movie)
            .Where(s => s.StartTime > DateTime.UtcNow) // Only future showtimes
            .OrderBy(s => s.StartTime)
            .Take(3)
            .ToListAsync(cancellationToken);

        if (!users.Any() || !showtimes.Any())
            return; // Cannot seed tickets without users and showtimes

        var tickets = new List<Ticket>();
        var ticketIdCounter = 1;

        // For each showtime, get a random seat
        foreach (var showtime in showtimes)
        {
            var seats = await context.Seats
                .Where(s => s.HallId == showtime.HallId)
                .Take(3) // Get first 3 seats
                .ToListAsync(cancellationToken);

            if (!seats.Any())
                continue;

            var user = users[ticketIdCounter % users.Count]; // Rotate through users
            var seat = seats[0]; // Use first available seat

            // Calculate final price: BasePrice × PriceMultiplier
            var finalPrice = showtime.BasePrice * seat.PriceMultiplier;

            // Create different ticket statuses for testing
            var status = ticketIdCounter switch
            {
                1 => TicketStatus.Confirmed,  // First ticket: Confirmed (paid)
                2 => TicketStatus.Pending,    // Second ticket: Pending (awaiting payment, recent)
                3 => TicketStatus.Expired,    // Third ticket: Expired (older than 10 minutes)
                _ => TicketStatus.Pending
            };

            // Set creation time based on status
            var createdAt = status switch
            {
                TicketStatus.Confirmed => DateTime.UtcNow.AddHours(-2),  // Confirmed 2 hours ago
                TicketStatus.Pending => DateTime.UtcNow.AddMinutes(-5),  // Pending 5 minutes ago (within 10-min window)
                TicketStatus.Expired => DateTime.UtcNow.AddMinutes(-15), // Expired 15 minutes ago (beyond 10-min window)
                _ => DateTime.UtcNow
            };

            var ticket = new Ticket
            {
                Id = new Guid($"TICKET0{ticketIdCounter}-0001-0001-0001-000000000001"),
                UserId = user.Id,
                ShowtimeId = showtime.Id,
                SeatId = seat.Id,
                TicketNumber = GenerateTicketNumber(ticketIdCounter),
                HolderName = $"{user.FirstName} {user.LastName}",
                Price = finalPrice,
                Status = status,
                CreatedAt = createdAt
            };

            tickets.Add(ticket);
            ticketIdCounter++;
        }

        if (tickets.Any())
        {
            await context.Tickets.AddRangeAsync(tickets, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    /// <summary>
    /// Generates a unique ticket number in format TKT-YYYYMMDD-XXXX.
    /// </summary>
    private static string GenerateTicketNumber(int counter)
    {
        var dateStr = DateTime.UtcNow.ToString("yyyyMMdd");
        return $"TKT-{dateStr}-{counter:D4}";
    }
}

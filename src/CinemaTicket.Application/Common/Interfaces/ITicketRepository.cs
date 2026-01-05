using CinemaTicket.Domain.Entities;
using CinemaTicket.Application.Common.Interfaces;

public interface ITicketRepository : IRepository<Ticket>
{
    /// <summary>
    /// Checks whether a seat is already booked for a specific showtime.
    /// </summary>
    Task<bool> ExistsAsync(
        Guid showtimeId,
        Guid seatId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all tickets for a specific showtime.
    /// </summary>
    Task<List<Ticket>> GetByShowtimeAsync(
        Guid showtimeId,
        CancellationToken cancellationToken = default);
}

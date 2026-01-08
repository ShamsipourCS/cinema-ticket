using CinemaTicket.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CinemaTicket.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<Movie> Movies { get; }
    DbSet<Cinema> Cinemas { get; }
    DbSet<Hall> Halls { get; }
    DbSet<Seat> Seats { get; }
    DbSet<Showtime> Showtimes { get; }
    DbSet<Ticket> Tickets { get; }
    DbSet<Payment> Payments { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

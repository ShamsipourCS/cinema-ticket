using System;
using System.Threading;
using System.Threading.Tasks;

namespace CinemaTicket.Domain.Interfaces;

/// <summary>
/// Coordinates work across multiple repositories and provides transaction support.
/// This is the single entry point for data access operations.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    // Repository Access - Add properties as repositories are created
    IUserRepository Users { get; }
    IRefreshTokenRepository RefreshTokens { get; }
    IMovieRepository Movies { get; }
    ICinemaRepository Cinemas { get; }
    IHallRepository Halls { get; }
    // IShowtimeRepository Showtimes { get; }
    ITicketRepository Tickets { get; }
    IPaymentRepository Payments { get; }

    /// <summary>
    /// Saves all pending changes to the database.
    /// Call this after all repository operations are complete.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begins a database transaction for multiple operations.
    /// Use when you need all-or-nothing behavior across SaveChanges calls.
    /// </summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commits the current transaction.
    /// </summary>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rolls back the current transaction.
    /// </summary>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}

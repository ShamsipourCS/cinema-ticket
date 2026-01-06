using CinemaTicket.Domain.Entities;

namespace CinemaTicket.Domain.Interfaces;

/// <summary>
/// Repository interface for User-specific database operations.
/// </summary>
public interface IUserRepository : IRepository<User>
{
    /// <summary>
    /// Gets a user by their email address asynchronously.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The user if found, otherwise null.</returns>
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user by their email address with their refresh tokens included.
    /// Useful for authentication scenarios to avoid N+1 query problems.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The user with refresh tokens if found, otherwise null.</returns>
    Task<User?> GetByEmailWithRefreshTokensAsync(string email, CancellationToken cancellationToken = default);
}

using CinemaTicket.Domain.Entities;

namespace CinemaTicket.Domain.Interfaces;

/// <summary>
/// Repository interface for RefreshToken-specific database operations.
/// </summary>
public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
    /// <summary>
    /// Gets a valid (non-revoked, non-expired) refresh token for a specific user.
    /// </summary>
    /// <param name="token">The token string to search for.</param>
    /// <param name="userId">The ID of the user who owns the token.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The refresh token if found and valid, otherwise null.</returns>
    Task<RefreshToken?> GetValidTokenAsync(string token, Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a refresh token by its token string value.
    /// </summary>
    /// <param name="token">The token string to search for.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The refresh token if found, otherwise null.</returns>
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Revokes all active refresh tokens for a specific user.
    /// Useful for logout-all-devices or security breach scenarios.
    /// </summary>
    /// <param name="userId">The ID of the user whose tokens should be revoked.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RevokeAllUserTokensAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes all expired refresh tokens from the database.
    /// Should be called periodically for cleanup (e.g., via background service).
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of tokens removed.</returns>
    Task<int> RemoveExpiredTokensAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active (non-revoked, non-expired) refresh tokens for a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user whose tokens should be retrieved.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of active refresh tokens.</returns>
    Task<IEnumerable<RefreshToken>> GetActiveTokensForUserAsync(Guid userId, CancellationToken cancellationToken = default);
}

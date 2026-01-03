using CinemaTicket.Domain.Common;

namespace CinemaTicket.Domain.Entities;

/// <summary>
/// Represents a refresh token for maintaining user sessions beyond access token expiration.
/// </summary>
/// <remarks>
/// Refresh tokens allow users to obtain new access tokens without re-authenticating.
/// Security features:
/// - Globally unique token value (enforced by database unique index)
/// - Expiration tracking with UTC timestamps
/// - Revocation capability for security breaches or logout
/// - Audit trail via AuditableEntity (CreatedAt timestamp)
/// Tokens should be stored securely and never exposed in logs or client-side code.
/// </remarks>
public class RefreshToken : AuditableEntity
{
    /// <summary>
    /// Gets or sets the ID of the user this token belongs to.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the cryptographically secure token string.
    /// </summary>
    /// <remarks>
    /// This value is a base64-encoded 256-bit random token generated using
    /// cryptographically secure random number generation. It must be globally
    /// unique across all users (enforced by database unique constraint).
    /// Never expose this value in logs or error messages.
    /// </remarks>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the expiration date and time of the token in UTC.
    /// </summary>
    /// <remarks>
    /// IMPORTANT: This value MUST be stored in UTC to ensure consistent
    /// expiration checks across different time zones and server locations.
    /// Always use DateTime.UtcNow when setting this value, never DateTime.Now.
    /// Tokens are considered expired when DateTime.UtcNow > ExpiresAt.
    /// </remarks>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the token has been revoked.
    /// </summary>
    /// <remarks>
    /// Revoked tokens cannot be used to obtain new access tokens, even if
    /// they haven't expired yet. Tokens should be revoked when:
    /// - User logs out (revoke current token)
    /// - User changes password (revoke all tokens for security)
    /// - Security breach detected (revoke all tokens)
    /// - User explicitly requests session termination
    /// Always check both IsRevoked and ExpiresAt before accepting a refresh token.
    /// </remarks>
    public bool IsRevoked { get; set; }
    
    /// <summary>
    /// Gets or sets the user this refresh token is associated with.
    /// </summary>
    public User? User { get; set; }
}

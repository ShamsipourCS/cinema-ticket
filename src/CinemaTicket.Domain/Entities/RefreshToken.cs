using CinemaTicket.Domain.Common;

namespace CinemaTicket.Domain.Entities;

/// <summary>
/// Represents a refresh token for a user session.
/// </summary>
public class RefreshToken : AuditableEntity
{
    /// <summary>
    /// Gets or sets the ID of the user this token belongs to.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the token string.
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the expiration date and time of the token.
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the token has been revoked.
    /// </summary>
    public bool IsRevoked { get; set; }
    
    /// <summary>
    /// Gets or sets the user this refresh token is associated with.
    /// </summary>
    public User? User { get; set; }
}

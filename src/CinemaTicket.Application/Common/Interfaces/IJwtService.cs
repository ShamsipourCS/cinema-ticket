using System.Security.Claims;
using CinemaTicket.Domain.Entities;

namespace CinemaTicket.Application.Common.Interfaces;

/// <summary>
/// Service for handling JSON Web Token (JWT) operations.
/// Logs security-related events for monitoring and auditing purposes.
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// Generates a JWT access token for the specified user.
    /// Logs token generation operations at Debug level.
    /// </summary>
    /// <param name="user">The user to generate the token for.</param>
    /// <returns>A JWT token string.</returns>
    string GenerateToken(User user);

    /// <summary>
    /// Generates a cryptographically strong random refresh token.
    /// </summary>
    /// <returns>A base64 encoded refresh token string.</returns>
    string GenerateRefreshToken();

    /// <summary>
    /// Extracts the claims principal from an expired JWT token.
    /// Logs warnings for invalid tokens and errors for unexpected failures.
    /// </summary>
    /// <param name="token">The expired JWT token.</param>
    /// <returns>The extracted claims principal, or null if the token is invalid.</returns>
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}

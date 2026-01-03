using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CinemaTicket.Application.Common.Interfaces;
using CinemaTicket.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace CinemaTicket.Infrastructure.Services;

/// <summary>
/// Implementation of the IJwtService for handling JWT token generation and validation.
/// Uses HS256 symmetric key encryption with configurable expiration and strict validation.
/// All tokens use UTC timestamps to ensure consistency across time zones.
/// </summary>
/// <remarks>
/// Security considerations:
/// - Secret key must be at least 32 characters for HS256 security
/// - Issuer and audience validation prevent token reuse across applications
/// - Algorithm validation uses case-sensitive comparison to prevent bypass attacks
/// - Refresh tokens use cryptographically secure random number generation
/// </remarks>
public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<JwtService> _logger;

    private const int DefaultExpirationMinutes = 60;
    private const int MinimumKeyLength = 32;

    /// <summary>
    /// Initializes a new instance of the JwtService class.
    /// </summary>
    /// <param name="configuration">The application configuration.</param>
    /// <param name="logger">The logger instance.</param>
    public JwtService(IConfiguration configuration, ILogger<JwtService> logger)
    {
        _configuration = configuration;
        _logger = logger;

        // Validate configuration on startup
        ValidateConfiguration();
    }

    /// <summary>
    /// Generates a JWT access token for the specified user.
    /// </summary>
    /// <param name="user">The user to generate the token for.</param>
    /// <returns>A signed JWT token string containing user claims (ID, email, role).</returns>
    /// <remarks>
    /// Token includes:
    /// - User ID (NameIdentifier claim)
    /// - Email address (Email claim)
    /// - User role (Role claim)
    /// - Expiration timestamp in UTC (configurable, defaults to 60 minutes)
    /// - Issuer and Audience for validation
    /// Token is signed using HS256 algorithm with the configured secret key.
    /// </remarks>
    public string GenerateToken(User user)
    {
        _logger.LogDebug("Generating JWT token for user {UserId}", user.Id);

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:Secret"]!);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(GetExpirationMinutes()),
            Issuer = _configuration["JwtSettings:Issuer"],
            Audience = _configuration["JwtSettings:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        _logger.LogDebug("Successfully generated JWT token for user {UserId}", user.Id);
        return tokenHandler.WriteToken(token);
    }

    /// <summary>
    /// Generates a cryptographically secure random refresh token.
    /// </summary>
    /// <returns>A base64-encoded 256-bit random token string.</returns>
    /// <remarks>
    /// Uses RandomNumberGenerator (cryptographically secure RNG) to generate
    /// a 32-byte (256-bit) random value, then base64-encodes it for storage.
    /// This token should be stored with a user reference and expiration time
    /// in the database, and must be globally unique (enforced by unique index).
    /// </remarks>
    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    /// <summary>
    /// Validates an expired JWT token and extracts the claims principal.
    /// </summary>
    /// <param name="token">The expired JWT token to validate.</param>
    /// <returns>
    /// The ClaimsPrincipal extracted from the token if validation succeeds,
    /// or null if the token is invalid or malformed.
    /// </returns>
    /// <remarks>
    /// This method is specifically designed for refresh token flows where the
    /// access token has expired but we still need to verify it was legitimately
    /// issued by this application. Validation includes:
    /// - Issuer and Audience verification (prevents cross-application token reuse)
    /// - Signature verification (ensures token wasn't tampered with)
    /// - Algorithm verification using case-sensitive comparison (prevents algorithm confusion attacks)
    /// - Lifetime validation is DISABLED (since we expect expired tokens)
    /// Returns null on SecurityTokenException (logs warning), throws on unexpected errors.
    /// </remarks>
    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidIssuer = _configuration["JwtSettings:Issuer"],
            ValidAudience = _configuration["JwtSettings:Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]!)),
            ValidateLifetime = false // Here we are validating an expired token
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.Ordinal))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
        catch (SecurityTokenException ex)
        {
            _logger.LogWarning(ex, "Invalid security token provided");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error validating expired token");
            throw;
        }
    }

    private void ValidateConfiguration()
    {
        var secret = _configuration["JwtSettings:Secret"];
        if (string.IsNullOrEmpty(secret))
            throw new InvalidOperationException("JwtSettings:Secret is required");

        if (secret.Length < MinimumKeyLength)
            throw new InvalidOperationException($"JwtSettings:Secret must be at least {MinimumKeyLength} characters");

        if (string.IsNullOrEmpty(_configuration["JwtSettings:Issuer"]))
            throw new InvalidOperationException("JwtSettings:Issuer is required");

        if (string.IsNullOrEmpty(_configuration["JwtSettings:Audience"]))
            throw new InvalidOperationException("JwtSettings:Audience is required");
    }

    private int GetExpirationMinutes()
    {
        var expirationString = _configuration["JwtSettings:ExpirationInMinutes"];
        if (string.IsNullOrEmpty(expirationString))
        {
            _logger.LogWarning("JwtSettings:ExpirationInMinutes not configured, using default: {DefaultMinutes}", DefaultExpirationMinutes);
            return DefaultExpirationMinutes;
        }

        if (!int.TryParse(expirationString, out var minutes) || minutes <= 0)
        {
            _logger.LogWarning("Invalid JwtSettings:ExpirationInMinutes value: {Value}, using default: {DefaultMinutes}",
                expirationString, DefaultExpirationMinutes);
            return DefaultExpirationMinutes;
        }

        return minutes;
    }
}

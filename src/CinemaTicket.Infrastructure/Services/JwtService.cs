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
/// </summary>
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

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

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

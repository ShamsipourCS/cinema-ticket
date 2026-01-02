using System.Security.Claims;
using CinemaTicket.Domain.Entities;

namespace CinemaTicket.Application.Common.Interfaces;

public interface IJwtService
{
    Task<string> GenerateToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}

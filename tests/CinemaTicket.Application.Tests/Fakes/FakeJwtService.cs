using System.Security.Claims;
using System.Threading;
using CinemaTicket.Application.Common.Interfaces;
using CinemaTicket.Domain.Entities;

namespace CinemaTicket.Application.Tests.Fakes;

public sealed class FakeJwtService : IJwtService
{
    private int _refreshCounter;

    public ClaimsPrincipal? PrincipalToReturn { get; set; }

    public string GenerateToken(User user)
        => $"access-token-{user.Id}";

    public string GenerateRefreshToken()
        => $"refresh-token-{Interlocked.Increment(ref _refreshCounter)}";

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        => PrincipalToReturn;
}

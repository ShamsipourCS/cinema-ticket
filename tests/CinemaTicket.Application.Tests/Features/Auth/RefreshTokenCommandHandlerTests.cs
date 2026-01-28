using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using CinemaTicket.Application.Features.Auth.Commands.RefreshToken;
using CinemaTicket.Application.Tests.Fakes;
using CinemaTicket.Domain.Entities;

namespace CinemaTicket.Application.Tests.Features.Auth;

public class RefreshTokenCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidToken_RotatesRefreshToken()
    {
        // ARRANGE
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Email = "refresh@example.com",
            FirstName = "Refresh",
            LastName = "User"
        };

        var users = new FakeUserRepository();
        users.Seed(user);

        var refreshTokens = new FakeRefreshTokenRepository();
        var existingToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = "refresh-token-1",
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        };
        refreshTokens.Seed(existingToken);

        var jwt = new FakeJwtService
        {
            PrincipalToReturn = new ClaimsPrincipal(
                new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                }))
        };

        var uow = new FakeUnitOfWork(
            users,
            refreshTokens,
            new FakePaymentRepository(),
            new FakeMovieRepository(),
            new FakeCinemaRepository(),
            new FakeHallRepository());

        var handler = new RefreshTokenCommandHandler(uow, jwt);

        // ACT
        var result = await handler.Handle(new RefreshTokenCommand("expired-access-token", "refresh-token-1"), CancellationToken.None);

        // ASSERT
        Assert.StartsWith("access-token-", result.AccessToken);
        Assert.Matches(@"^refresh-token-\d+$", result.RefreshToken);
        Assert.True(existingToken.IsRevoked);

        var storedTokens = (await refreshTokens.GetAllAsync()).ToList();
        Assert.Equal(2, storedTokens.Count);
        Assert.Equal(1, uow.SaveChangesCalls);
    }

    [Fact]
    public async Task Handle_InvalidRefreshToken_ThrowsUnauthorized()
    {
        // ARRANGE
        var userId = Guid.NewGuid();
        var users = new FakeUserRepository();
        users.Seed(new User { Id = userId, Email = "refresh@example.com" });

        var jwt = new FakeJwtService
        {
            PrincipalToReturn = new ClaimsPrincipal(
                new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                }))
        };

        var uow = new FakeUnitOfWork(
            users,
            new FakeRefreshTokenRepository(),
            new FakePaymentRepository(),
            new FakeMovieRepository(),
            new FakeCinemaRepository(),
            new FakeHallRepository());

        var handler = new RefreshTokenCommandHandler(uow, jwt);

        // ACT + ASSERT
        await Assert.ThrowsAsync<CinemaTicket.Domain.Exceptions.UnauthorizedAccessException>(() =>
            handler.Handle(new RefreshTokenCommand("expired-access-token", "missing-token"), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ExpiredRefreshToken_ThrowsUnauthorized()
    {
        // ARRANGE
        var userId = Guid.NewGuid();
        var users = new FakeUserRepository();
        users.Seed(new User { Id = userId, Email = "refresh@example.com" });

        var refreshTokens = new FakeRefreshTokenRepository();
        var expiredToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = "expired-refresh-token",
            ExpiresAt = DateTime.UtcNow.AddDays(-1), // Expired 1 day ago
            IsRevoked = false
        };
        refreshTokens.Seed(expiredToken);

        var jwt = new FakeJwtService
        {
            PrincipalToReturn = new ClaimsPrincipal(
                new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                }))
        };

        var uow = new FakeUnitOfWork(
            users,
            refreshTokens,
            new FakePaymentRepository(),
            new FakeMovieRepository(),
            new FakeCinemaRepository(),
            new FakeHallRepository());

        var handler = new RefreshTokenCommandHandler(uow, jwt);

        // ACT + ASSERT
        await Assert.ThrowsAsync<CinemaTicket.Domain.Exceptions.UnauthorizedAccessException>(() =>
            handler.Handle(new RefreshTokenCommand("expired-access-token", "expired-refresh-token"), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_RevokedRefreshToken_ThrowsUnauthorized()
    {
        // ARRANGE
        var userId = Guid.NewGuid();
        var users = new FakeUserRepository();
        users.Seed(new User { Id = userId, Email = "refresh@example.com" });

        var refreshTokens = new FakeRefreshTokenRepository();
        var revokedToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = "revoked-refresh-token",
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = true // Revoked
        };
        refreshTokens.Seed(revokedToken);

        var jwt = new FakeJwtService
        {
            PrincipalToReturn = new ClaimsPrincipal(
                new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                }))
        };

        var uow = new FakeUnitOfWork(
            users,
            refreshTokens,
            new FakePaymentRepository(),
            new FakeMovieRepository(),
            new FakeCinemaRepository(),
            new FakeHallRepository());

        var handler = new RefreshTokenCommandHandler(uow, jwt);

        // ACT + ASSERT
        await Assert.ThrowsAsync<CinemaTicket.Domain.Exceptions.UnauthorizedAccessException>(() =>
            handler.Handle(new RefreshTokenCommand("expired-access-token", "revoked-refresh-token"), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ValidToken_OldTokenIsRevoked()
    {
        // ARRANGE
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Email = "refresh@example.com",
            FirstName = "Refresh",
            LastName = "User"
        };

        var users = new FakeUserRepository();
        users.Seed(user);

        var refreshTokens = new FakeRefreshTokenRepository();
        var existingToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = "old-refresh-token",
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        };
        refreshTokens.Seed(existingToken);

        var jwt = new FakeJwtService
        {
            PrincipalToReturn = new ClaimsPrincipal(
                new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                }))
        };

        var uow = new FakeUnitOfWork(
            users,
            refreshTokens,
            new FakePaymentRepository(),
            new FakeMovieRepository(),
            new FakeCinemaRepository(),
            new FakeHallRepository());

        var handler = new RefreshTokenCommandHandler(uow, jwt);

        // ACT
        await handler.Handle(new RefreshTokenCommand("expired-access-token", "old-refresh-token"), CancellationToken.None);

        // ASSERT - Verify old token was revoked
        var oldToken = await refreshTokens.GetByTokenAsync("old-refresh-token");
        Assert.NotNull(oldToken);
        Assert.True(oldToken.IsRevoked);
    }

    [Fact]
    public async Task Handle_MultipleActiveTokens_OnlyUsedTokenIsRevoked()
    {
        // ARRANGE
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Email = "refresh@example.com",
            FirstName = "Refresh",
            LastName = "User"
        };

        var users = new FakeUserRepository();
        users.Seed(user);

        var refreshTokens = new FakeRefreshTokenRepository();

        // User has multiple active tokens (e.g., logged in from different devices)
        var token1 = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = "device1-token",
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        };
        var token2 = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = "device2-token",
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        };
        refreshTokens.Seed(token1, token2);

        var jwt = new FakeJwtService
        {
            PrincipalToReturn = new ClaimsPrincipal(
                new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                }))
        };

        var uow = new FakeUnitOfWork(
            users,
            refreshTokens,
            new FakePaymentRepository(),
            new FakeMovieRepository(),
            new FakeCinemaRepository(),
            new FakeHallRepository());

        var handler = new RefreshTokenCommandHandler(uow, jwt);

        // ACT - Use token from device1
        await handler.Handle(new RefreshTokenCommand("expired-access-token", "device1-token"), CancellationToken.None);

        // ASSERT
        var usedToken = await refreshTokens.GetByTokenAsync("device1-token");
        var otherToken = await refreshTokens.GetByTokenAsync("device2-token");

        Assert.True(usedToken!.IsRevoked); // Used token should be revoked
        Assert.False(otherToken!.IsRevoked); // Other device's token should remain valid

        var allTokens = (await refreshTokens.GetAllAsync()).ToList();
        Assert.Equal(3, allTokens.Count); // 2 old + 1 new
    }
}

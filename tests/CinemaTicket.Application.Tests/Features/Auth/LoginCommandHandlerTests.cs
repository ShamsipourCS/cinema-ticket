using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CinemaTicket.Application.Features.Auth.Commands.Login;
using CinemaTicket.Application.Tests.Fakes;
using CinemaTicket.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace CinemaTicket.Application.Tests.Features.Auth;

public class LoginCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidCredentials_ReturnsTokens()
    {
        // ARRANGE
        var users = new FakeUserRepository();
        var passwordHasher = new PasswordHasher<User>();
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "login@example.com",
            FirstName = "Login",
            LastName = "User"
        };
        user.PasswordHash = passwordHasher.HashPassword(user, "CorrectPassword!");
        users.Seed(user);

        var refreshTokens = new FakeRefreshTokenRepository();
        var uow = new FakeUnitOfWork(
            users,
            refreshTokens,
            new FakePaymentRepository(),
            new FakeMovieRepository(),
            new FakeCinemaRepository(),
            new FakeHallRepository());

        var handler = new LoginCommandHandler(uow, new FakeJwtService(), passwordHasher);

        // ACT
        var result = await handler.Handle(new LoginCommand("login@example.com", "CorrectPassword!"), CancellationToken.None);

        // ASSERT
        Assert.StartsWith("access-token-", result.AccessToken);
        Assert.Matches(@"^refresh-token-\d+$", result.RefreshToken);

        var storedTokens = (await refreshTokens.GetAllAsync()).ToList();
        Assert.Single(storedTokens);
        Assert.Equal(1, uow.SaveChangesCalls);
    }

    [Fact]
    public async Task Handle_InvalidPassword_ThrowsUnauthorized()
    {
        // ARRANGE
        var users = new FakeUserRepository();
        var passwordHasher = new PasswordHasher<User>();
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "login@example.com",
            FirstName = "Login",
            LastName = "User"
        };
        user.PasswordHash = passwordHasher.HashPassword(user, "CorrectPassword!");
        users.Seed(user);

        var uow = new FakeUnitOfWork(
            users,
            new FakeRefreshTokenRepository(),
            new FakePaymentRepository(),
            new FakeMovieRepository(),
            new FakeCinemaRepository(),
            new FakeHallRepository());

        var handler = new LoginCommandHandler(uow, new FakeJwtService(), passwordHasher);

        // ACT + ASSERT
        await Assert.ThrowsAsync<CinemaTicket.Domain.Exceptions.UnauthorizedAccessException>(() =>
            handler.Handle(new LoginCommand("login@example.com", "WrongPassword"), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsUnauthorized()
    {
        // ARRANGE
        var users = new FakeUserRepository();
        var uow = new FakeUnitOfWork(
            users,
            new FakeRefreshTokenRepository(),
            new FakePaymentRepository(),
            new FakeMovieRepository(),
            new FakeCinemaRepository(),
            new FakeHallRepository());

        var handler = new LoginCommandHandler(uow, new FakeJwtService(), new PasswordHasher<User>());

        // ACT + ASSERT
        await Assert.ThrowsAsync<CinemaTicket.Domain.Exceptions.UnauthorizedAccessException>(() =>
            handler.Handle(new LoginCommand("nonexistent@example.com", "AnyPassword"), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_EmailCaseInsensitive_ReturnsTokens()
    {
        // ARRANGE
        var users = new FakeUserRepository();
        var passwordHasher = new PasswordHasher<User>();
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "login@example.com",
            FirstName = "Login",
            LastName = "User"
        };
        user.PasswordHash = passwordHasher.HashPassword(user, "CorrectPassword!");
        users.Seed(user);

        var refreshTokens = new FakeRefreshTokenRepository();
        var uow = new FakeUnitOfWork(
            users,
            refreshTokens,
            new FakePaymentRepository(),
            new FakeMovieRepository(),
            new FakeCinemaRepository(),
            new FakeHallRepository());

        var handler = new LoginCommandHandler(uow, new FakeJwtService(), passwordHasher);

        // ACT - Login with different case
        var result = await handler.Handle(new LoginCommand("LOGIN@EXAMPLE.COM", "CorrectPassword!"), CancellationToken.None);

        // ASSERT
        Assert.StartsWith("access-token-", result.AccessToken);
        Assert.Matches(@"^refresh-token-\d+$", result.RefreshToken);
    }
}

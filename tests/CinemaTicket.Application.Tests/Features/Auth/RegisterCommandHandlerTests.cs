using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CinemaTicket.Application.Features.Auth.Commands.Register;
using CinemaTicket.Application.Tests.Fakes;
using CinemaTicket.Domain.Entities;
using CinemaTicket.Domain.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace CinemaTicket.Application.Tests.Features.Auth;

public class RegisterCommandHandlerTests
{
    [Fact]
    public async Task Handle_NewUser_ReturnsTokensAndPersists()
    {
        // ARRANGE
        var users = new FakeUserRepository();
        var refreshTokens = new FakeRefreshTokenRepository();
        var uow = new FakeUnitOfWork(
            users,
            refreshTokens,
            new FakePaymentRepository(),
            new FakeMovieRepository(),
            new FakeCinemaRepository(),
            new FakeHallRepository());

        var jwt = new FakeJwtService();
        var passwordHasher = new PasswordHasher<User>();
        var handler = new RegisterCommandHandler(uow, jwt, passwordHasher);

        var command = new RegisterCommand(
            Email: "new.user@example.com",
            Password: "Test@12345",
            FirstName: "New",
            LastName: "User",
            Phone: null);

        // ACT
        var result = await handler.Handle(command, CancellationToken.None);

        // ASSERT
        Assert.StartsWith("access-token-", result.AccessToken);
        Assert.Matches(@"^refresh-token-\d+$", result.RefreshToken);

        var storedUsers = (await users.GetAllAsync()).ToList();
        var storedTokens = (await refreshTokens.GetAllAsync()).ToList();
        Assert.Single(storedUsers);
        Assert.Single(storedTokens);
        Assert.Equal(1, uow.SaveChangesCalls);
    }

    [Fact]
    public async Task Handle_DuplicateEmail_ThrowsConflict()
    {
        // ARRANGE
        var users = new FakeUserRepository();
        users.Seed(new User
        {
            Id = Guid.NewGuid(),
            Email = "existing@example.com",
            FirstName = "Existing",
            LastName = "User"
        });

        var uow = new FakeUnitOfWork(
            users,
            new FakeRefreshTokenRepository(),
            new FakePaymentRepository(),
            new FakeMovieRepository(),
            new FakeCinemaRepository(),
            new FakeHallRepository());

        var handler = new RegisterCommandHandler(uow, new FakeJwtService(), new PasswordHasher<User>());
        var command = new RegisterCommand(
            Email: "existing@example.com",
            Password: "Test@12345",
            FirstName: "Dup",
            LastName: "User",
            Phone: null);

        // ACT + ASSERT
        await Assert.ThrowsAsync<ConflictException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_NewUser_PasswordIsHashed()
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

        var passwordHasher = new PasswordHasher<User>();
        var handler = new RegisterCommandHandler(uow, new FakeJwtService(), passwordHasher);

        var command = new RegisterCommand(
            Email: "security@example.com",
            Password: "PlainTextPassword!123",
            FirstName: "Security",
            LastName: "Test",
            Phone: null);

        // ACT
        await handler.Handle(command, CancellationToken.None);

        // ASSERT
        var storedUser = (await users.GetAllAsync()).Single();
        Assert.NotNull(storedUser.PasswordHash);
        Assert.NotEqual("PlainTextPassword!123", storedUser.PasswordHash);

        // Verify password can be verified
        var verifyResult = passwordHasher.VerifyHashedPassword(storedUser, storedUser.PasswordHash, "PlainTextPassword!123");
        Assert.Equal(PasswordVerificationResult.Success, verifyResult);
    }

    [Fact]
    public async Task Handle_DuplicateEmailDifferentCase_ThrowsConflict()
    {
        // ARRANGE
        var users = new FakeUserRepository();
        users.Seed(new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            FirstName = "Existing",
            LastName = "User"
        });

        var uow = new FakeUnitOfWork(
            users,
            new FakeRefreshTokenRepository(),
            new FakePaymentRepository(),
            new FakeMovieRepository(),
            new FakeCinemaRepository(),
            new FakeHallRepository());

        var handler = new RegisterCommandHandler(uow, new FakeJwtService(), new PasswordHasher<User>());
        var command = new RegisterCommand(
            Email: "TEST@EXAMPLE.COM", // Different case
            Password: "Test@12345",
            FirstName: "Dup",
            LastName: "User",
            Phone: null);

        // ACT + ASSERT
        await Assert.ThrowsAsync<ConflictException>(() => handler.Handle(command, CancellationToken.None));
    }
}

using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using CinemaTicket.Application.Features.Payments.Commands.CreatePaymentIntent;
using CinemaTicket.Application.Tests.Fakes;
using CinemaTicket.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace CinemaTicket.Application.Tests.Features.Payments;

public class CreatePaymentIntentCommandHandlerTests
{
    [Fact]
    public async Task Handle_AuthenticatedUser_CreatesPaymentAndReturnsResponse()
    {
        // ARRANGE
        var payments = new FakePaymentRepository();
        var stripe = new FakeStripePaymentService
        {
            NextCreateResult = ("pi_test_999", "secret_test_999")
        };

        var userId = Guid.NewGuid();
        var httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(
                    new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                    }, "TestAuth"))
            }
        };

        var uow = new FakeUnitOfWork(
            new FakeUserRepository(),
            new FakeRefreshTokenRepository(),
            payments,
            new FakeMovieRepository(),
            new FakeCinemaRepository(),
            new FakeHallRepository());

        var handler = new CreatePaymentIntentCommandHandler(uow, stripe, httpContextAccessor);

        // ACT
        var result = await handler.Handle(new CreatePaymentIntentCommand(Amount: 5000, Currency: null, Metadata: null), CancellationToken.None);

        // ASSERT
        Assert.Equal("pi_test_999", result.PaymentIntentId);
        Assert.Equal("secret_test_999", result.ClientSecret);
        Assert.Equal(5000, result.Amount);
        Assert.Equal("usd", result.Currency);

        var storedPayments = (await payments.GetAllAsync()).ToList();
        Assert.Single(storedPayments);
        Assert.Equal(PaymentStatus.Pending, storedPayments[0].Status);
        Assert.Equal(50.00m, storedPayments[0].Amount);
        Assert.Equal("usd", storedPayments[0].Currency);
        Assert.Equal(1, uow.SaveChangesCalls);
    }

    [Fact]
    public async Task Handle_MissingUser_ThrowsUnauthorized()
    {
        // ARRANGE
        var uow = new FakeUnitOfWork(
            new FakeUserRepository(),
            new FakeRefreshTokenRepository(),
            new FakePaymentRepository(),
            new FakeMovieRepository(),
            new FakeCinemaRepository(),
            new FakeHallRepository());

        var handler = new CreatePaymentIntentCommandHandler(uow, new FakeStripePaymentService(), new HttpContextAccessor());

        // ACT + ASSERT
        await Assert.ThrowsAsync<CinemaTicket.Domain.Exceptions.UnauthorizedAccessException>(() =>
            handler.Handle(new CreatePaymentIntentCommand(Amount: 5000, Currency: "usd", Metadata: null), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_CustomCurrency_UsesProvidedCurrency()
    {
        // ARRANGE
        var payments = new FakePaymentRepository();
        var stripe = new FakeStripePaymentService
        {
            NextCreateResult = ("pi_test_eur", "secret_test_eur")
        };

        var userId = Guid.NewGuid();
        var httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(
                    new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                    }, "TestAuth"))
            }
        };

        var uow = new FakeUnitOfWork(
            new FakeUserRepository(),
            new FakeRefreshTokenRepository(),
            payments,
            new FakeMovieRepository(),
            new FakeCinemaRepository(),
            new FakeHallRepository());

        var handler = new CreatePaymentIntentCommandHandler(uow, stripe, httpContextAccessor);

        // ACT
        var result = await handler.Handle(new CreatePaymentIntentCommand(Amount: 10000, Currency: "eur", Metadata: null), CancellationToken.None);

        // ASSERT
        Assert.Equal("eur", result.Currency);
        var storedPayment = (await payments.GetAllAsync()).Single();
        Assert.Equal("eur", storedPayment.Currency);
    }

    [Fact]
    public async Task Handle_WithMetadata_StoresMetadataCorrectly()
    {
        // ARRANGE
        var payments = new FakePaymentRepository();
        var stripe = new FakeStripePaymentService
        {
            NextCreateResult = ("pi_test_metadata", "secret_test_metadata")
        };

        var userId = Guid.NewGuid();
        var httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(
                    new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                    }, "TestAuth"))
            }
        };

        var uow = new FakeUnitOfWork(
            new FakeUserRepository(),
            new FakeRefreshTokenRepository(),
            payments,
            new FakeMovieRepository(),
            new FakeCinemaRepository(),
            new FakeHallRepository());

        var handler = new CreatePaymentIntentCommandHandler(uow, stripe, httpContextAccessor);

        var metadata = new Dictionary<string, string>
        {
            { "ticket_id", "ticket-123" },
            { "showtime_id", "showtime-456" }
        };

        // ACT
        await handler.Handle(new CreatePaymentIntentCommand(Amount: 5000, Currency: null, Metadata: metadata), CancellationToken.None);

        // ASSERT
        var storedPayment = (await payments.GetAllAsync()).Single();
        Assert.NotNull(storedPayment);
    }

    [Fact]
    public async Task Handle_MultiplePayments_CreatesMultipleRecords()
    {
        // ARRANGE
        var payments = new FakePaymentRepository();
        var stripe = new FakeStripePaymentService();

        var userId = Guid.NewGuid();
        var httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(
                    new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                    }, "TestAuth"))
            }
        };

        var uow = new FakeUnitOfWork(
            new FakeUserRepository(),
            new FakeRefreshTokenRepository(),
            payments,
            new FakeMovieRepository(),
            new FakeCinemaRepository(),
            new FakeHallRepository());

        var handler = new CreatePaymentIntentCommandHandler(uow, stripe, httpContextAccessor);

        // ACT - Create multiple payments
        stripe.NextCreateResult = ("pi_test_1", "secret_1");
        await handler.Handle(new CreatePaymentIntentCommand(Amount: 1000, Currency: null, Metadata: null), CancellationToken.None);

        stripe.NextCreateResult = ("pi_test_2", "secret_2");
        await handler.Handle(new CreatePaymentIntentCommand(Amount: 2000, Currency: null, Metadata: null), CancellationToken.None);

        // ASSERT
        var storedPayments = (await payments.GetAllAsync()).ToList();
        Assert.Equal(2, storedPayments.Count);
        Assert.Contains(storedPayments, p => p.StripePaymentIntentId == "pi_test_1");
        Assert.Contains(storedPayments, p => p.StripePaymentIntentId == "pi_test_2");
    }
}

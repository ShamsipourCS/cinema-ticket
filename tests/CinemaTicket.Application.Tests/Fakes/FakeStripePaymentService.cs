using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CinemaTicket.Application.Common.Interfaces;

namespace CinemaTicket.Application.Tests.Fakes;

public sealed class FakeStripePaymentService : IStripePaymentService
{
    public (string PaymentIntentId, string ClientSecret) NextCreateResult { get; set; } =
        ("pi_test_123", "secret_test_123");

    public string NextStatus { get; set; } = "succeeded";
    public bool NextConfirmResult { get; set; } = true;
    public bool NextCancelResult { get; set; } = true;
    public (string EventId, string EventType, string? PaymentIntentId) NextWebhookResult { get; set; } =
        ("evt_test_123", "payment_intent.succeeded", "pi_test_123");

    public Task<(string PaymentIntentId, string ClientSecret)> CreatePaymentIntentAsync(
        long amount,
        string? currency = null,
        Dictionary<string, string>? metadata = null,
        CancellationToken cancellationToken = default)
        => Task.FromResult(NextCreateResult);

    public Task<string> GetPaymentIntentStatusAsync(string paymentIntentId, CancellationToken cancellationToken = default)
        => Task.FromResult(NextStatus);

    public Task<bool> ConfirmPaymentIntentAsync(string paymentIntentId, CancellationToken cancellationToken = default)
        => Task.FromResult(NextConfirmResult);

    public Task<bool> CancelPaymentIntentAsync(string paymentIntentId, CancellationToken cancellationToken = default)
        => Task.FromResult(NextCancelResult);

    public (string EventId, string EventType, string? PaymentIntentId) VerifyWebhookSignature(
        string json,
        string stripeSignatureHeader)
        => NextWebhookResult;
}

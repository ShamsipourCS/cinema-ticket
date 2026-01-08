namespace CinemaTicket.Application.Features.Payments.DTOs;

/// <summary>
/// Data transfer object for Stripe payment intent responses.
/// </summary>
/// <param name="PaymentIntentId">The Stripe payment intent ID.</param>
/// <param name="ClientSecret">The client secret for confirming the payment on the frontend.</param>
/// <param name="Amount">The payment amount in cents.</param>
/// <param name="Currency">The payment currency (e.g., "usd").</param>
public sealed record PaymentIntentResponseDto(
    string PaymentIntentId,
    string ClientSecret,
    long Amount,
    string Currency
);

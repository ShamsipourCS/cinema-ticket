namespace CinemaTicket.Application.Common.Interfaces;

/// <summary>
/// Service interface for Stripe payment processing operations.
/// Handles payment intent creation, confirmation, status retrieval, and cancellation.
/// All operations are async and support cancellation tokens for graceful shutdown.
/// </summary>
public interface IStripePaymentService
{
    /// <summary>
    /// Creates a Stripe payment intent for a ticket booking.
    /// </summary>
    /// <param name="amount">The payment amount in the smallest currency unit (e.g., cents for USD, pence for GBP).</param>
    /// <param name="currency">The currency code (e.g., "usd", "eur", "gbp"). If null, uses the configured default currency.</param>
    /// <param name="metadata">Optional metadata to attach to the payment intent (e.g., ticketId, userId, showtimeId).</param>
    /// <param name="cancellationToken">Cancellation token for request cancellation.</param>
    /// <returns>
    /// A tuple containing:
    /// - PaymentIntentId: The Stripe payment intent ID (starts with pi_) for server-side tracking
    /// - ClientSecret: The client secret for confirming the payment on the frontend
    /// </returns>
    /// <exception cref="Stripe.StripeException">Thrown when Stripe API request fails.</exception>
    /// <remarks>
    /// The client secret should be passed to the frontend to complete the payment using Stripe.js or mobile SDKs.
    /// Metadata is useful for reconciliation and can be queried in the Stripe Dashboard.
    /// </remarks>
    Task<(string PaymentIntentId, string ClientSecret)> CreatePaymentIntentAsync(
        long amount,
        string? currency = null,
        Dictionary<string, string>? metadata = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the current status of a payment intent from Stripe.
    /// </summary>
    /// <param name="paymentIntentId">The Stripe payment intent ID (starts with pi_).</param>
    /// <param name="cancellationToken">Cancellation token for request cancellation.</param>
    /// <returns>
    /// The payment intent status string. Common values include:
    /// - "requires_payment_method": Waiting for payment method
    /// - "requires_confirmation": Payment method attached, needs confirmation
    /// - "requires_action": Requires additional action (e.g., 3D Secure authentication)
    /// - "processing": Payment is being processed
    /// - "succeeded": Payment completed successfully
    /// - "canceled": Payment intent was canceled
    /// </returns>
    /// <exception cref="Stripe.StripeException">Thrown when Stripe API request fails or payment intent not found.</exception>
    /// <remarks>
    /// Use this method to check payment status before updating ticket status or processing refunds.
    /// </remarks>
    Task<string> GetPaymentIntentStatusAsync(
        string paymentIntentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Confirms a payment intent on the server side.
    /// </summary>
    /// <param name="paymentIntentId">The Stripe payment intent ID to confirm.</param>
    /// <param name="cancellationToken">Cancellation token for request cancellation.</param>
    /// <returns>
    /// True if the payment was successfully confirmed (status is "succeeded"),
    /// false if confirmation failed or requires additional action.
    /// </returns>
    /// <exception cref="Stripe.StripeException">Thrown when Stripe API request fails.</exception>
    /// <remarks>
    /// Server-side confirmation is useful for automated payment processing.
    /// For payments requiring 3D Secure or other authentication, use client-side confirmation instead.
    /// </remarks>
    Task<bool> ConfirmPaymentIntentAsync(
        string paymentIntentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancels a payment intent.
    /// </summary>
    /// <param name="paymentIntentId">The Stripe payment intent ID to cancel.</param>
    /// <param name="cancellationToken">Cancellation token for request cancellation.</param>
    /// <returns>
    /// True if the payment intent was successfully canceled,
    /// false if cancellation failed (e.g., payment already succeeded).
    /// </returns>
    /// <exception cref="Stripe.StripeException">Thrown when Stripe API request fails.</exception>
    /// <remarks>
    /// Payment intents can only be canceled if they haven't been successfully completed.
    /// Use this when a booking is canceled during the reservation timeout period.
    /// Canceled payment intents cannot be reused.
    /// </remarks>
    Task<bool> CancelPaymentIntentAsync(
        string paymentIntentId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifies the signature of a Stripe webhook event and returns the event type and payment intent ID.
    /// </summary>
    /// <param name="json">The raw JSON payload from the webhook request body.</param>
    /// <param name="stripeSignatureHeader">The Stripe-Signature header value from the webhook request.</param>
    /// <returns>A tuple containing the event ID, event type, and payment intent ID if signature verification succeeds.</returns>
    /// <exception cref="System.UnauthorizedAccessException">Thrown when signature verification fails.</exception>
    /// <exception cref="System.InvalidOperationException">Thrown when webhook secret is not configured.</exception>
    /// <remarks>
    /// This method uses the configured webhook secret to verify that the webhook event came from Stripe.
    /// ALWAYS verify webhook signatures before processing events to prevent fake webhook attacks.
    /// The Stripe-Signature header contains the timestamp and signature used for verification.
    /// </remarks>
    (string EventId, string EventType, string? PaymentIntentId) VerifyWebhookSignature(string json, string stripeSignatureHeader);
}

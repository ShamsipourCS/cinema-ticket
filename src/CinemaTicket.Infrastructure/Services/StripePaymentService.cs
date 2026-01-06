using CinemaTicket.Application.Common.Interfaces;
using CinemaTicket.Infrastructure.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;

namespace CinemaTicket.Infrastructure.Services;

/// <summary>
/// Implementation of the IStripePaymentService for handling Stripe payment operations.
/// Uses Stripe.net SDK to interact with Stripe API for payment intent management.
/// All operations use async/await and support cancellation tokens.
/// </summary>
/// <remarks>
/// Security and operational considerations:
/// - API key is set globally via StripeConfiguration.ApiKey on service initialization
/// - All amounts are in smallest currency unit (cents) to avoid decimal precision issues
/// - Comprehensive logging at Debug (operations), Warning (validation), and Error (failures) levels
/// - Configuration is validated on startup to fail fast if Stripe keys are missing or invalid
/// - All Stripe API exceptions are caught, logged, and re-thrown for middleware handling
/// </remarks>
public class StripePaymentService : IStripePaymentService
{
    private readonly StripeSettings _settings;
    private readonly ILogger<StripePaymentService> _logger;

    private const int MinimumAmount = 50; // Minimum 50 cents ($0.50 USD)
    private const string TestKeyPrefix = "sk_test_";
    private const string LiveKeyPrefix = "sk_live_";

    /// <summary>
    /// Initializes a new instance of the StripePaymentService class.
    /// </summary>
    /// <param name="stripeOptions">The Stripe configuration options.</param>
    /// <param name="logger">The logger instance.</param>
    public StripePaymentService(
        IOptions<StripeSettings> stripeOptions,
        ILogger<StripePaymentService> logger)
    {
        _settings = stripeOptions.Value;
        _logger = logger;

        // Set Stripe API key globally for all Stripe.net SDK operations
        StripeConfiguration.ApiKey = _settings.ApiKey;

        // Validate configuration on startup to fail fast
        ValidateConfiguration();

        _logger.LogInformation("StripePaymentService initialized with API key prefix: {KeyPrefix}",
            _settings.ApiKey.StartsWith(TestKeyPrefix) ? "TEST" : "LIVE");
    }

    /// <summary>
    /// Creates a Stripe payment intent for a ticket booking.
    /// </summary>
    /// <param name="amount">The payment amount in the smallest currency unit (e.g., cents for USD).</param>
    /// <param name="currency">The currency code (e.g., "usd"). If null, uses the configured default currency.</param>
    /// <param name="metadata">Optional metadata to attach to the payment intent.</param>
    /// <param name="cancellationToken">Cancellation token for request cancellation.</param>
    /// <returns>A tuple containing the payment intent ID and client secret for frontend confirmation.</returns>
    /// <exception cref="ArgumentException">Thrown when amount is less than the minimum allowed.</exception>
    /// <exception cref="StripeException">Thrown when Stripe API request fails.</exception>
    public async Task<(string PaymentIntentId, string ClientSecret)> CreatePaymentIntentAsync(
        long amount,
        string? currency = null,
        Dictionary<string, string>? metadata = null,
        CancellationToken cancellationToken = default)
    {
        if (amount < MinimumAmount)
        {
            _logger.LogWarning("Payment amount {Amount} is below minimum {MinimumAmount}", amount, MinimumAmount);
            throw new ArgumentException($"Payment amount must be at least {MinimumAmount} cents", nameof(amount));
        }

        var effectiveCurrency = currency ?? _settings.Currency;
        _logger.LogDebug("Creating payment intent for amount {Amount} {Currency}", amount, effectiveCurrency);

        try
        {
            var service = new PaymentIntentService();
            var options = new PaymentIntentCreateOptions
            {
                Amount = amount,
                Currency = effectiveCurrency,
                Metadata = metadata,
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true
                }
            };

            var paymentIntent = await service.CreateAsync(options, cancellationToken: cancellationToken);

            _logger.LogDebug("Successfully created payment intent {PaymentIntentId} with status {Status}",
                paymentIntent.Id, paymentIntent.Status);

            return (paymentIntent.Id, paymentIntent.ClientSecret);
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe API error creating payment intent: {ErrorType} - {ErrorMessage} (Code: {ErrorCode})",
                ex.StripeError?.Type, ex.StripeError?.Message, ex.StripeError?.Code);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating payment intent");
            throw;
        }
    }

    /// <summary>
    /// Retrieves the current status of a payment intent from Stripe.
    /// </summary>
    /// <param name="paymentIntentId">The Stripe payment intent ID.</param>
    /// <param name="cancellationToken">Cancellation token for request cancellation.</param>
    /// <returns>The payment intent status string.</returns>
    /// <exception cref="ArgumentException">Thrown when paymentIntentId is null or whitespace.</exception>
    /// <exception cref="StripeException">Thrown when Stripe API request fails or payment intent not found.</exception>
    public async Task<string> GetPaymentIntentStatusAsync(
        string paymentIntentId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(paymentIntentId))
        {
            throw new ArgumentException("Payment intent ID cannot be null or empty", nameof(paymentIntentId));
        }

        _logger.LogDebug("Retrieving status for payment intent {PaymentIntentId}", paymentIntentId);

        try
        {
            var service = new PaymentIntentService();
            var paymentIntent = await service.GetAsync(paymentIntentId, cancellationToken: cancellationToken);

            _logger.LogDebug("Payment intent {PaymentIntentId} has status {Status}",
                paymentIntentId, paymentIntent.Status);

            return paymentIntent.Status;
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe API error retrieving payment intent {PaymentIntentId}: {ErrorType} - {ErrorMessage}",
                paymentIntentId, ex.StripeError?.Type, ex.StripeError?.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error retrieving payment intent {PaymentIntentId}", paymentIntentId);
            throw;
        }
    }

    /// <summary>
    /// Confirms a payment intent on the server side.
    /// </summary>
    /// <param name="paymentIntentId">The Stripe payment intent ID to confirm.</param>
    /// <param name="cancellationToken">Cancellation token for request cancellation.</param>
    /// <returns>True if the payment was successfully confirmed, false otherwise.</returns>
    /// <exception cref="ArgumentException">Thrown when paymentIntentId is null or whitespace.</exception>
    /// <exception cref="StripeException">Thrown when Stripe API request fails.</exception>
    public async Task<bool> ConfirmPaymentIntentAsync(
        string paymentIntentId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(paymentIntentId))
        {
            throw new ArgumentException("Payment intent ID cannot be null or empty", nameof(paymentIntentId));
        }

        _logger.LogDebug("Confirming payment intent {PaymentIntentId}", paymentIntentId);

        try
        {
            var service = new PaymentIntentService();
            var paymentIntent = await service.ConfirmAsync(paymentIntentId, cancellationToken: cancellationToken);

            var succeeded = paymentIntent.Status == "succeeded";

            if (succeeded)
            {
                _logger.LogInformation("Payment intent {PaymentIntentId} confirmed successfully", paymentIntentId);
            }
            else
            {
                _logger.LogWarning("Payment intent {PaymentIntentId} confirmation resulted in status {Status}",
                    paymentIntentId, paymentIntent.Status);
            }

            return succeeded;
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe API error confirming payment intent {PaymentIntentId}: {ErrorType} - {ErrorMessage}",
                paymentIntentId, ex.StripeError?.Type, ex.StripeError?.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error confirming payment intent {PaymentIntentId}", paymentIntentId);
            throw;
        }
    }

    /// <summary>
    /// Cancels a payment intent.
    /// </summary>
    /// <param name="paymentIntentId">The Stripe payment intent ID to cancel.</param>
    /// <param name="cancellationToken">Cancellation token for request cancellation.</param>
    /// <returns>True if the payment intent was successfully canceled, false otherwise.</returns>
    /// <exception cref="ArgumentException">Thrown when paymentIntentId is null or whitespace.</exception>
    /// <exception cref="StripeException">Thrown when Stripe API request fails.</exception>
    public async Task<bool> CancelPaymentIntentAsync(
        string paymentIntentId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(paymentIntentId))
        {
            throw new ArgumentException("Payment intent ID cannot be null or empty", nameof(paymentIntentId));
        }

        _logger.LogDebug("Canceling payment intent {PaymentIntentId}", paymentIntentId);

        try
        {
            var service = new PaymentIntentService();
            var paymentIntent = await service.CancelAsync(paymentIntentId, cancellationToken: cancellationToken);

            var canceled = paymentIntent.Status == "canceled";

            if (canceled)
            {
                _logger.LogInformation("Payment intent {PaymentIntentId} canceled successfully", paymentIntentId);
            }
            else
            {
                _logger.LogWarning("Payment intent {PaymentIntentId} cancellation resulted in status {Status}",
                    paymentIntentId, paymentIntent.Status);
            }

            return canceled;
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe API error canceling payment intent {PaymentIntentId}: {ErrorType} - {ErrorMessage}",
                paymentIntentId, ex.StripeError?.Type, ex.StripeError?.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error canceling payment intent {PaymentIntentId}", paymentIntentId);
            throw;
        }
    }

    /// <summary>
    /// Validates the Stripe configuration on service initialization.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when configuration is invalid or missing.</exception>
    private void ValidateConfiguration()
    {
        if (string.IsNullOrWhiteSpace(_settings.ApiKey))
        {
            throw new InvalidOperationException("StripeSettings:ApiKey is required");
        }

        if (!_settings.ApiKey.StartsWith(TestKeyPrefix) && !_settings.ApiKey.StartsWith(LiveKeyPrefix))
        {
            throw new InvalidOperationException(
                $"StripeSettings:ApiKey must start with '{TestKeyPrefix}' (test mode) or '{LiveKeyPrefix}' (live mode)");
        }

        if (string.IsNullOrWhiteSpace(_settings.WebhookSecret))
        {
            _logger.LogWarning("StripeSettings:WebhookSecret is not configured. Webhook signature verification will not be available.");
        }

        if (string.IsNullOrWhiteSpace(_settings.Currency))
        {
            throw new InvalidOperationException("StripeSettings:Currency is required");
        }

        if (_settings.Currency.Length != 3)
        {
            throw new InvalidOperationException("StripeSettings:Currency must be a 3-letter ISO currency code (e.g., 'usd')");
        }

        _logger.LogDebug("Stripe configuration validated successfully");
    }
}

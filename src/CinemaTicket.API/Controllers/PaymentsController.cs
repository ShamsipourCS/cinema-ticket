using CinemaTicket.Application.Features.Payments.Commands.CreatePaymentIntent;
using CinemaTicket.Application.Features.Payments.Commands.ProcessWebhook;
using CinemaTicket.Application.Features.Payments.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaTicket.API.Controllers;

/// <summary>
/// Controller for payment-related operations (Stripe integration).
/// All endpoints require authentication.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PaymentsController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="PaymentsController"/> class.
    /// </summary>
    /// <param name="mediator">The MediatR instance for sending commands.</param>
    /// <param name="logger">The logger instance.</param>
    public PaymentsController(IMediator mediator, ILogger<PaymentsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Creates a Stripe payment intent for ticket booking.
    /// Requires authentication via Bearer token.
    /// </summary>
    /// <param name="command">The payment intent command containing amount and currency.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Payment intent response with client secret for frontend confirmation.</returns>
    /// <response code="200">Payment intent created successfully.</response>
    /// <response code="400">Validation failed (e.g., amount below minimum).</response>
    /// <response code="401">Not authenticated.</response>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/Payments/intent
    ///     Authorization: Bearer {access_token}
    ///     {
    ///         "amount": 2500,
    ///         "currency": "usd",
    ///         "metadata": {
    ///             "showtimeId": "guid",
    ///             "seatId": "guid"
    ///         }
    ///     }
    ///
    /// Notes:
    /// - Amount is in smallest currency unit (cents for USD)
    /// - Minimum amount: 50 cents ($0.50 USD)
    /// - Default currency: "usd"
    /// - User ID is automatically added to metadata from authentication context
    /// </remarks>
    [HttpPost("intent")]
    [ProducesResponseType(typeof(PaymentIntentResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreatePaymentIntent(
        [FromBody] CreatePaymentIntentCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Webhook endpoint for Stripe event notifications.
    /// Processes payment status changes asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>200 OK if event processed successfully, 400 if signature verification fails.</returns>
    /// <response code="200">Webhook event processed successfully.</response>
    /// <response code="400">Invalid webhook signature.</response>
    /// <remarks>
    /// This endpoint is called by Stripe when payment events occur.
    /// It verifies the webhook signature before processing to ensure authenticity.
    ///
    /// Supported events:
    /// - payment_intent.succeeded: Payment completed successfully
    /// - payment_intent.payment_failed: Payment failed
    /// - payment_intent.canceled: Payment was canceled
    ///
    /// Configuration:
    /// 1. Set up webhook endpoint in Stripe Dashboard: https://your-api-domain/api/payments/webhook
    /// 2. Configure webhook secret in appsettings.json: StripeSettings:WebhookSecret
    /// 3. For local testing, use Stripe CLI: stripe listen --forward-to https://localhost:7xxx/api/payments/webhook
    /// </remarks>
    [HttpPost("webhook")]
    [AllowAnonymous] // Webhooks come from Stripe, not authenticated users
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> StripeWebhook(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received Stripe webhook event");

        try
        {
            // Read raw request body (required for signature verification)
            using var reader = new StreamReader(Request.Body);
            var json = await reader.ReadToEndAsync(cancellationToken);

            // Get Stripe signature from header
            var signature = Request.Headers["Stripe-Signature"].ToString();

            if (string.IsNullOrEmpty(signature))
            {
                _logger.LogWarning("Stripe webhook received without signature header");
                return BadRequest(new { Error = "Missing Stripe-Signature header" });
            }

            // Send to handler via MediatR
            var command = new ProcessWebhookCommand(json, signature);
            var success = await _mediator.Send(command, cancellationToken);

            if (success)
            {
                _logger.LogInformation("Webhook processed successfully");
                return Ok();
            }

            return BadRequest(new { Error = "Webhook processing failed" });
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Webhook signature verification failed");
            return BadRequest(new { Error = "Invalid webhook signature" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing webhook");
            // Return 500 so Stripe retries the webhook
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { Error = "Internal server error processing webhook" });
        }
    }
}

using CinemaTicket.Application.Features.Payments.Commands.CreatePaymentIntent;
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
}

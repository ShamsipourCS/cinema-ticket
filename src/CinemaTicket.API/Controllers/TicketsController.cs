using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CinemaTicket.Application.Features.Bookings.Commands.CreateBooking;
using CinemaTicket.Application.Features.Bookings.Commands.ConfirmBooking;
using CinemaTicket.Application.Features.Bookings.DTOs;

namespace CinemaTicket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class TicketsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TicketsController(IMediator mediator) => _mediator = mediator;

    public sealed record CreateBookingRequest(
        // UserId removed - derived from JWT authentication to prevent user impersonation
        Guid ShowtimeId,
        Guid SeatId,
        string HolderName
    );

    [HttpPost("bookings")]
    [Authorize]  // Require authentication to prevent unauthorized booking creation
    [ProducesResponseType(typeof(BookingResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<BookingResultDto>> CreateBooking([FromBody] CreateBookingRequest request, CancellationToken ct)
    {
        // Extract userId from JWT claims to prevent user impersonation
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized("Invalid or missing user authentication");
        }

        var result = await _mediator.Send(new CreateBookingCommand(
            userId,  // Use authenticated user ID from JWT, not request body
            request.ShowtimeId,
            request.SeatId,
            request.HolderName
        ), ct);

        return Ok(result);
    }

    public sealed record ConfirmBookingRequest(
        string StripePaymentIntentId,
        Guid ShowtimeId,
        Guid SeatId,
        string HolderName
    );

    /// <summary>
    /// Confirms a booking after successful Stripe payment.
    /// Validates payment status, creates confirmed ticket, and links payment to ticket.
    /// </summary>
    [HttpPost("confirm-booking")]
    [Authorize]
    [ProducesResponseType(typeof(BookingResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<BookingResultDto>> ConfirmBooking(
        [FromBody] ConfirmBookingRequest request,
        CancellationToken ct)
    {
        // Extract userId from JWT claims
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized("Invalid or missing user authentication");
        }

        try
        {
            var command = new ConfirmBookingCommand(
                request.StripePaymentIntentId,
                userId,
                request.ShowtimeId,
                request.SeatId,
                request.HolderName
            );

            var result = await _mediator.Send(command, ct);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            // This includes payment not succeeded, seat taken, price mismatch, etc.
            return BadRequest(new { Error = ex.Message });
        }
    }
}

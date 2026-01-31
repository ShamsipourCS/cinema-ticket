using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CinemaTicket.Application.Features.Bookings.Commands.ConfirmBooking;
using CinemaTicket.Application.Features.Bookings.Commands.CreateBooking;
using CinemaTicket.Application.Features.Bookings.DTOs;

namespace CinemaTicket.API.Controllers;

[ApiController]
[Route("api/bookings")]
[Authorize]
public sealed class BookingsController : ControllerBase
{
    private readonly IMediator _mediator;
    public BookingsController(IMediator mediator) => _mediator = mediator;

    private bool TryGetUserId(out Guid userId)
    {
        userId = default;
        var v = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return !string.IsNullOrWhiteSpace(v) && Guid.TryParse(v, out userId);
    }

    public sealed record CreateBookingRequest(Guid ShowtimeId, Guid SeatId, string HolderName);

    [HttpPost]
    [ProducesResponseType(typeof(BookingResultDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<BookingResultDto>> Create([FromBody] CreateBookingRequest request, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized("Invalid user.");

        var result = await _mediator.Send(
            new CreateBookingCommand(userId, request.ShowtimeId, request.SeatId, request.HolderName),
            ct);

        return Ok(result);
    }

    public sealed record ConfirmBookingRequest(string StripePaymentIntentId, Guid ShowtimeId, Guid SeatId, string HolderName);

    [HttpPost("confirm")]
    [ProducesResponseType(typeof(BookingResultDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<BookingResultDto>> Confirm([FromBody] ConfirmBookingRequest request, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized("Invalid user.");

        var result = await _mediator.Send(
            new ConfirmBookingCommand(request.StripePaymentIntentId, userId, request.ShowtimeId, request.SeatId, request.HolderName),
            ct);

        return Ok(result);
    }
}

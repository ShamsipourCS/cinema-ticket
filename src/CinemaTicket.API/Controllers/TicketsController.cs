using MediatR;
using Microsoft.AspNetCore.Mvc;
using CinemaTicket.Application.Features.Bookings.Commands.CreateBooking;
using CinemaTicket.Application.Features.Bookings.DTOs;

namespace CinemaTicket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class TicketsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TicketsController(IMediator mediator) => _mediator = mediator;

    public sealed record CreateBookingRequest(
        Guid UserId,
        Guid ShowtimeId,
        Guid SeatId,
        string HolderName
    );

    [HttpPost("bookings")]
    [ProducesResponseType(typeof(BookingResultDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<BookingResultDto>> CreateBooking([FromBody] CreateBookingRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateBookingCommand(
            request.UserId,
            request.ShowtimeId,
            request.SeatId,
            request.HolderName
        ), ct);

        return Ok(result);
    }
}

using MediatR;
using Microsoft.AspNetCore.Mvc;
using CinemaTicket.Application.Features.Seats.DTOs;
using CinemaTicket.Application.Features.Seats.Queries.GetAvailableSeats;

namespace CinemaTicket.API.Controllers;

[ApiController]
[Route("api/showtimes/{showtimeId:guid}/seats")]
public sealed class SeatsController : ControllerBase
{
    private readonly IMediator _mediator;
    public SeatsController(IMediator mediator) => _mediator = mediator;

    [HttpGet("available")]
    [ProducesResponseType(typeof(IReadOnlyList<SeatAvailabilityDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<SeatAvailabilityDto>>> GetAvailable(Guid showtimeId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAvailableSeatsQuery(showtimeId), ct);
        return Ok(result);
    }
}

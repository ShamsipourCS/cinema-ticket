using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CinemaTicket.Application.Features.Showtimes.Commands.CreateShowtime;
using CinemaTicket.Application.Features.Showtimes.Commands.UpdateShowtime;
using CinemaTicket.Application.Features.Showtimes.Commands.DeleteShowtime;
using CinemaTicket.Application.Features.Showtimes.DTOs;
using CinemaTicket.Application.Features.Showtimes.Queries.GetShowtimes;
using CinemaTicket.Application.Features.Showtimes.Queries.GetShowtimeById;

namespace CinemaTicket.API.Controllers;

[ApiController]
[Route("api/showtimes")]
public sealed class ShowtimesController : ControllerBase
{
    private readonly IMediator _mediator;
    public ShowtimesController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ShowtimeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ShowtimeDto>>> GetShowtimes(
        [FromQuery] Guid? movieId,
        [FromQuery] Guid? hallId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] bool? isActive,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new GetShowtimesQuery(movieId, hallId, from, to, isActive), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ShowtimeDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ShowtimeDto>> GetById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetShowtimeByIdQuery(id), ct);
        return Ok(result);
    }

    public sealed record CreateShowtimeRequest(
        Guid MovieId,
        Guid HallId,
        DateTime StartTime,
        DateTime EndTime,
        decimal BasePrice
    );

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ShowtimeDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ShowtimeDto>> Create([FromBody] CreateShowtimeRequest request, CancellationToken ct)
    {
        var cmd = new CreateShowtimeCommand(
            request.MovieId,
            request.HallId,
            request.StartTime,
            request.EndTime,
            request.BasePrice
        );

        var result = await _mediator.Send(cmd, ct);
        return Ok(result);
    }

    public sealed record UpdateShowtimeRequest(
        Guid MovieId,
        Guid HallId,
        DateTime StartsAt,
        DateTime EndsAt,
        decimal Price,
        bool IsActive
    );

    [HttpPut("{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(typeof(ShowtimeDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<ShowtimeDto>> Update(Guid id, [FromBody] UpdateShowtimeRequest request, CancellationToken ct)
    {
        var cmd = new UpdateShowtimeCommand
        {
            ShowtimeId = id,
            MovieId = request.MovieId,
            HallId = request.HallId,
            StartsAt = request.StartsAt,
            EndsAt = request.EndsAt,
            Price = request.Price,
            IsActive = request.IsActive
        };

        var result = await _mediator.Send(cmd, ct);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteShowtimeCommand(id), ct);
        return NoContent();
    }
}

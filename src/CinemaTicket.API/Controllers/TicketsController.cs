using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CinemaTicket.Application.Features.Tickets.DTOs;
using CinemaTicket.Application.Features.Tickets.Queries.GetMyTickets;
using CinemaTicket.Application.Features.Tickets.Queries.GetTicketById;
using CinemaTicket.Application.Features.Tickets.Commands.CancelTicket;

namespace CinemaTicket.API.Controllers;

[ApiController]
[Route("api/tickets")]
[Authorize]
public sealed class TicketsController : ControllerBase
{
    private readonly IMediator _mediator;
    public TicketsController(IMediator mediator) => _mediator = mediator;

    private bool TryGetUserId(out Guid userId)
    {
        userId = default;
        var v = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return !string.IsNullOrWhiteSpace(v) && Guid.TryParse(v, out userId);
    }

    [HttpGet("me")]
    [ProducesResponseType(typeof(IReadOnlyList<TicketDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<TicketDto>>> MyTickets(CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized("Invalid or missing user authentication.");

        var result = await _mediator.Send(new GetMyTicketsQuery(userId), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "TicketOwner")]
    [ProducesResponseType(typeof(TicketDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TicketDto>> GetById(Guid id, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized("Invalid or missing user authentication.");

        try
        {
            var result = await _mediator.Send(new GetTicketByIdQuery(userId, id), ct);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Error = ex.Message });
        }
    }

    [HttpPost("{id:guid}/cancel")]
    [Authorize(Policy = "TicketOwner")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken ct)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized("Invalid or missing user authentication.");

        try
        {
            await _mediator.Send(new CancelTicketCommand(userId, id), ct);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }
}

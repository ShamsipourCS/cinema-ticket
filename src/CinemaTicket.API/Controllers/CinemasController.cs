using CinemaTicket.Application.Features.Cinemas.Commands.CreateCinema;
using CinemaTicket.Application.Features.Cinemas.Commands.DeleteCinema;
using CinemaTicket.Application.Features.Cinemas.Commands.UpdateCinema;
using CinemaTicket.Application.Features.Cinemas.Queries.GetCinemaById;
using CinemaTicket.Application.Features.Cinemas.Queries.GetCinemas;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaTicket.API.Controllers;

/// <summary>
/// Controller for cinema-related operations.
/// Demonstrates MediatR pipeline behaviors (ValidationBehavior and LoggingBehavior) in action.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CinemasController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CinemasController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CinemasController"/> class.
    /// </summary>
    /// <param name="mediator">The MediatR instance for sending queries and commands.</param>
    /// <param name="logger">The logger instance.</param>
    public CinemasController(IMediator mediator, ILogger<CinemasController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Gets a list of cinemas with optional filtering and pagination.
    /// </summary>
    /// <param name="city">Optional city filter.</param>
    /// <param name="pageNumber">Page number (default: 1, must be >= 1).</param>
    /// <param name="pageSize">Page size (default: 10, must be between 1 and 100).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of cinemas matching the query criteria.</returns>
    /// <response code="200">Returns the list of cinemas.</response>
    /// <response code="400">If validation fails (e.g., invalid page number or page size).</response>
    /// <remarks>
    /// Sample requests:
    ///
    ///     GET /api/Cinemas?pageNumber=1&amp;pageSize=10
    ///     GET /api/Cinemas?city=Tehran&amp;pageNumber=1&amp;pageSize=5
    ///
    /// Invalid request (will trigger ValidationBehavior):
    ///
    ///     GET /api/Cinemas?pageNumber=0&amp;pageSize=150
    ///
    /// This will return HTTP 400 with validation error messages.
    /// </remarks>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetCinemas(
        [FromQuery] string? city = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug(
            "GetCinemas request: City={City}, PageNumber={PageNumber}, PageSize={PageSize}",
            city, pageNumber, pageSize);

        var query = new GetCinemasQuery(city, pageNumber, pageSize);
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Gets a cinema by its unique identifier.
    /// </summary>
    /// <param name="id">Cinema id.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The requested cinema.</returns>
    /// <response code="200">Returns the cinema.</response>
    /// <response code="400">If validation fails.</response>
    /// <response code="404">If the cinema does not exist.</response>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /api/Cinemas/{id}
    /// </remarks>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCinemaById(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("GetCinemaById request: Id={Id}", id);

        try
        {
            var result = await _mediator.Send(new GetCinemaByIdQuery(id), cancellationToken);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Creates a new cinema.
    /// </summary>
    /// <param name="command">Cinema creation command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The id of the created cinema.</returns>
    /// <response code="201">Returns the created cinema id.</response>
    /// <response code="400">If validation fails.</response>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/Cinemas
    /// </remarks>
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCinema(
        [FromBody] CreateCinemaCommand command,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("CreateCinema request: Name={Name}, City={City}", command.Name, command.City);

        var id = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetCinemaById), new { id }, new { id });
    }

    /// <summary>
    /// Updates an existing cinema.
    /// </summary>
    /// <param name="id">Cinema id (route).</param>
    /// <param name="command">Cinema update command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="204">Cinema updated successfully.</response>
    /// <response code="400">If validation fails or route id does not match body id.</response>
    /// <response code="404">If the cinema does not exist.</response>
    /// <remarks>
    /// Sample request:
    ///
    ///     PUT /api/Cinemas/{id}
    /// </remarks>
    [HttpPut("{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateCinema(
        Guid id,
        [FromBody] UpdateCinemaCommand command,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("UpdateCinema request: RouteId={RouteId}, BodyId={BodyId}", id, command.Id);

        if (id != command.Id)
            return BadRequest("Route id must match command Id.");

        try
        {
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Deletes a cinema.
    /// </summary>
    /// <param name="id">Cinema id.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="204">Cinema deleted successfully.</response>
    /// <response code="400">If validation fails.</response>
    /// <response code="404">If the cinema does not exist.</response>
    /// <remarks>
    /// Sample request:
    ///
    ///     DELETE /api/Cinemas/{id}
    /// </remarks>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCinema(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("DeleteCinema request: Id={Id}", id);

        try
        {
            await _mediator.Send(new DeleteCinemaCommand(id), cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}

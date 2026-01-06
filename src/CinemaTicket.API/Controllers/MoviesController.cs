using CinemaTicket.Application.Features.Movies.Commands.CreateMovie;
using CinemaTicket.Application.Features.Movies.Commands.DeleteMovie;
using CinemaTicket.Application.Features.Movies.Commands.UpdateMovie;
using CinemaTicket.Application.Features.Movies.Queries.GetMovieById;
using CinemaTicket.Application.Features.Movies.Queries.GetMovies;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CinemaTicket.API.Controllers;

/// <summary>
/// Controller for movie-related operations.
/// Demonstrates MediatR pipeline behaviors (ValidationBehavior and LoggingBehavior) in action.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class MoviesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<MoviesController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="MoviesController"/> class.
    /// </summary>
    /// <param name="mediator">The MediatR instance for sending queries and commands.</param>
    /// <param name="logger">The logger instance.</param>
    public MoviesController(IMediator mediator, ILogger<MoviesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Gets a list of movies with optional filtering and pagination.
    /// </summary>
    /// <param name="genre">Optional genre filter.</param>
    /// <param name="pageNumber">Page number (default: 1, must be >= 1).</param>
    /// <param name="pageSize">Page size (default: 10, must be between 1 and 100).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of movies matching the query criteria.</returns>
    /// <response code="200">Returns the list of movies.</response>
    /// <response code="400">If validation fails (e.g., invalid page number or page size).</response>
    /// <remarks>
    /// Sample requests:
    ///
    ///     GET /api/movies?pageNumber=1&amp;pageSize=10
    ///     GET /api/movies?genre=Action&amp;pageNumber=1&amp;pageSize=5
    ///
    /// Invalid request (will trigger ValidationBehavior):
    ///
    ///     GET /api/movies?pageNumber=0&amp;pageSize=150
    ///
    /// This will return HTTP 400 with validation error messages.
    /// </remarks>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetMovies(
        [FromQuery] string? genre = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug(
            "GetMovies request: Genre={Genre}, PageNumber={PageNumber}, PageSize={PageSize}",
            genre, pageNumber, pageSize);

        var query = new GetMoviesQuery(genre, pageNumber, pageSize);
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Gets a movie by its unique identifier.
    /// </summary>
    /// <param name="id">Movie id.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The requested movie.</returns>
    /// <response code="200">Returns the movie.</response>
    /// <response code="400">If validation fails.</response>
    /// <response code="404">If the movie does not exist.</response>
    /// <remarks>
    /// Sample request:
    ///
    ///     GET /api/movies/{id}
    /// </remarks>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMovieById(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("GetMovieById request: Id={Id}", id);

        try
        {
            var result = await _mediator.Send(new GetMovieByIdQuery(id), cancellationToken);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    /// <summary>
    /// Creates a new movie.
    /// </summary>
    /// <param name="command">Movie creation command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The id of the created movie.</returns>
    /// <response code="201">Returns the created movie id.</response>
    /// <response code="400">If validation fails.</response>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/movies
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateMovie(
        [FromBody] CreateMovieCommand command,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("CreateMovie request: Title={Title}, Genre={Genre}", command.Title, command.Genre);

        var id = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetMovieById), new { id }, new { id });
    }

    /// <summary>
    /// Updates an existing movie.
    /// </summary>
    /// <param name="id">Movie id (route).</param>
    /// <param name="command">Movie update command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="204">Movie updated successfully.</response>
    /// <response code="400">If validation fails or route id does not match body id.</response>
    /// <response code="404">If the movie does not exist.</response>
    /// <remarks>
    /// Sample request:
    ///
    ///     PUT /api/movies/{id}
    /// </remarks>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateMovie(
        Guid id,
        [FromBody] UpdateMovieCommand command,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("UpdateMovie request: RouteId={RouteId}, BodyId={BodyId}", id, command.Id);

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
    /// Deletes a movie.
    /// </summary>
    /// <param name="id">Movie id.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="204">Movie deleted successfully.</response>
    /// <response code="400">If validation fails.</response>
    /// <response code="404">If the movie does not exist.</response>
    /// <remarks>
    /// Sample request:
    ///
    ///     DELETE /api/movies/{id}
    /// </remarks>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMovie(Guid id, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("DeleteMovie request: Id={Id}", id);

        try
        {
            await _mediator.Send(new DeleteMovieCommand(id), cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}

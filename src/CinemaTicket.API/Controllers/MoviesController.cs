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
    /// Sample request:
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
            genre,
            pageNumber,
            pageSize
        );

        var query = new GetMoviesQuery(genre, pageNumber, pageSize);

        // MediatR pipeline will execute:
        // 1. ValidationBehavior (validates query)
        // 2. LoggingBehavior (logs execution time)
        // 3. GetMoviesQueryHandler (executes business logic)
        var result = await _mediator.Send(query, cancellationToken);

        return Ok(result);
    }
}

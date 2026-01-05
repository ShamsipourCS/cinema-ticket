using CinemaTicket.Application.Features.Movies.DTOs;
using MediatR;

namespace CinemaTicket.Application.Features.Movies.Queries.GetMovies;

/// <summary>
/// Query to retrieve a list of movies with optional filtering and pagination.
/// </summary>
/// <param name="Genre">Optional genre filter. If provided, only movies of this genre are returned.</param>
/// <param name="PageNumber">The page number for pagination (must be >= 1).</param>
/// <param name="PageSize">The number of items per page (must be between 1 and 100).</param>
public record GetMoviesQuery(
    string? Genre = null,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<List<MovieDto>>;

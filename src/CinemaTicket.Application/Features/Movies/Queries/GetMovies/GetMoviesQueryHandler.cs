using CinemaTicket.Application.Common.Interfaces;
using CinemaTicket.Application.Features.Movies.DTOs;
using CinemaTicket.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CinemaTicket.Application.Features.Movies.Queries.GetMovies;

/// <summary>
/// Handler for the GetMoviesQuery that retrieves movies with optional filtering and pagination.
/// </summary>
public class GetMoviesQueryHandler : IRequestHandler<GetMoviesQuery, List<MovieDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetMoviesQueryHandler"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work for data access.</param>
    public GetMoviesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Handles the GetMoviesQuery request.
    /// </summary>
    /// <param name="request">The query request with filter and pagination parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of movie DTOs matching the query criteria.</returns>
    public async Task<List<MovieDto>> Handle(
        GetMoviesQuery request,
        CancellationToken cancellationToken)
    {
        // Access DbContext through UnitOfWork to query Movies
        // Note: In the future, this should use a proper IMovieRepository
        var context = _unitOfWork as DbContext;
        if (context == null)
        {
            throw new InvalidOperationException("UnitOfWork must be a DbContext instance");
        }

        // Build query with optional genre filter
        var query = context.Set<Movie>().AsQueryable();

        // Apply genre filter if provided
        if (!string.IsNullOrWhiteSpace(request.Genre))
        {
            query = query.Where(m => m.Genre == request.Genre);
        }

        // Apply pagination and execute query
        var movies = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        // Manual mapping to DTOs (AutoMapper will replace this later)
        return movies.Select(m => new MovieDto(
            Id: m.Id,
            Title: m.Title,
            Description: m.Description,
            DurationMinutes: m.DurationMinutes,
            Genre: m.Genre,
            Rating: m.Rating,
            PosterUrl: m.PosterUrl,
            ReleaseDate: m.ReleaseDate,
            IsActive: m.IsActive
        )).ToList();
    }
}

using CinemaTicket.Application.Common.Interfaces;
using CinemaTicket.Application.Features.Movies.DTOs;
using CinemaTicket.Domain.Entities;
using MediatR;

namespace CinemaTicket.Application.Features.Movies.Queries.GetMovies;

/// <summary>
/// Handler for the GetMoviesQuery that retrieves movies with optional filtering and pagination.
/// </summary>
public class GetMoviesQueryHandler : IRequestHandler<GetMoviesQuery, List<MovieDto>>
{
    private readonly IMovieRepository _movieRepository;

    public GetMoviesQueryHandler(IMovieRepository movieRepository)
    {
        _movieRepository = movieRepository;
    }

    public async Task<List<MovieDto>> Handle(GetMoviesQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Movie> movies;

        if (!string.IsNullOrWhiteSpace(request.Genre))
        {
            movies = await _movieRepository.FindAsync(
                m => m.Genre == request.Genre,
                cancellationToken);
        }
        else
        {
            movies = await _movieRepository.GetAllAsync(cancellationToken);
        }

        var paged = movies
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return paged.Select(m => new MovieDto(
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

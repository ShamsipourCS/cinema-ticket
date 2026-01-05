using CinemaTicket.Application.Common.Interfaces;
using CinemaTicket.Application.Features.Movies.DTOs;
using MediatR;

namespace CinemaTicket.Application.Features.Movies.Queries.GetMovieById;

public sealed class GetMovieByIdQueryHandler : IRequestHandler<GetMovieByIdQuery, MovieDto>
{
    private readonly IMovieRepository _movieRepository;

    public GetMovieByIdQueryHandler(IMovieRepository movieRepository)
    {
        _movieRepository = movieRepository;
    }

    public async Task<MovieDto> Handle(GetMovieByIdQuery request, CancellationToken cancellationToken)
    {
        var movie = await _movieRepository.GetByIdAsync(request.Id, cancellationToken);

        if (movie == null)
            throw new KeyNotFoundException($"Movie with id '{request.Id}' was not found.");

        return new MovieDto(
            Id: movie.Id,
            Title: movie.Title,
            Description: movie.Description,
            DurationMinutes: movie.DurationMinutes,
            Genre: movie.Genre,
            Rating: movie.Rating,
            PosterUrl: movie.PosterUrl,
            ReleaseDate: movie.ReleaseDate,
            IsActive: movie.IsActive
        );
    }
}

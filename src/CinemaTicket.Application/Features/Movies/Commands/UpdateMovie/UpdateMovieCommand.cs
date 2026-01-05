using MediatR;

namespace CinemaTicket.Application.Features.Movies.Commands.UpdateMovie;

public sealed record UpdateMovieCommand(
    Guid Id,
    string Title,
    string Description,
    int DurationMinutes,
    string Genre,
    string Rating,
    string PosterUrl,
    DateTime ReleaseDate,
    bool IsActive
) : IRequest;

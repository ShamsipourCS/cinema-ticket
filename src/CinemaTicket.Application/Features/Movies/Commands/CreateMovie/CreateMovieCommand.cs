using MediatR;

namespace CinemaTicket.Application.Features.Movies.Commands.CreateMovie;

public sealed record CreateMovieCommand(
    string Title,
    string Description,
    int DurationMinutes,
    string Genre,
    string Rating,
    string PosterUrl,
    DateTime ReleaseDate,
    bool IsActive
) : IRequest<Guid>;
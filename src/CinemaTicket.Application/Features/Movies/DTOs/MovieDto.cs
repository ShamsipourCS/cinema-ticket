namespace CinemaTicket.Application.Features.Movies.DTOs;

/// <summary>
/// Data transfer object for movie information.
/// </summary>
/// <param name="Id">The unique identifier for the movie.</param>
/// <param name="Title">The movie title.</param>
/// <param name="Description">The movie description.</param>
/// <param name="DurationMinutes">The duration of the movie in minutes.</param>
/// <param name="Genre">The genre of the movie.</param>
/// <param name="Rating">The content rating of the movie (e.g., PG-13, R).</param>
/// <param name="PosterUrl">The URL for the movie's poster image.</param>
/// <param name="ReleaseDate">The release date of the movie.</param>
/// <param name="IsActive">Indicates whether the movie is currently active or showing.</param>
public record MovieDto(
    Guid Id,
    string Title,
    string Description,
    int DurationMinutes,
    string Genre,
    string Rating,
    string PosterUrl,
    DateTime ReleaseDate,
    bool IsActive
);

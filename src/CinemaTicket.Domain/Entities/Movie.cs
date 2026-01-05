using CinemaTicket.Domain.Common;

namespace CinemaTicket.Domain.Entities;

/// <summary>
/// Represents a movie in the system.
/// </summary>
public class Movie : AuditableEntity
{
    /// <summary>
    /// Gets or sets the movie title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the movie description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the duration of the movie in minutes.
    /// </summary>
    public int DurationMinutes { get; set; }

    /// <summary>
    /// Gets or sets the genre of the movie.
    /// </summary>
    public string Genre { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the movie's content rating (e.g., PG-13, R).
    /// </summary>
    public string Rating { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the URL for the movie's poster image.
    /// </summary>
    public string PosterUrl { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the release date of the movie.
    /// </summary>
    public DateTime ReleaseDate { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the movie is currently active or showing.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets the collection of showtimes for this movie.
    /// </summary>
    public ICollection<Showtime> Showtimes { get; set; } = new List<Showtime>();
}

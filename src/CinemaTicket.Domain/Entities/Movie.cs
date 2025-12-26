using CinemaTicket.Domain.Common;

namespace CinemaTicket.Domain.Entities;

public class Movie : AuditableEntity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public string Genre { get; set; } = string.Empty;
    public string Rating { get; set; } = string.Empty; // e.g., PG-13, R
    public string PosterUrl { get; set; } = string.Empty;
    public DateTime ReleaseDate { get; set; }
    public bool IsActive { get; set; }

    // Navigation Properties
    // public ICollection<Showtime> Showtimes { get; set; } = new List<Showtime>();
}

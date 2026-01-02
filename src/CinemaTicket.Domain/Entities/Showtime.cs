using CinemaTicket.Domain.Common;

namespace CinemaTicket.Domain.Entities;

/// <summary>
/// Represents a scheduled showtime for a movie in a specific hall.
/// </summary>
public class Showtime : BaseEntity
{
    /// <summary>
    /// Gets or sets the ID of the movie to be shown.
    /// </summary>
    public Guid MovieId { get; set; }

    /// <summary>
    /// Gets or sets the ID of the hall where the movie will be shown.
    /// </summary>
    public Guid HallId { get; set; }

    /// <summary>
    /// Gets or sets the start time of the show.
    /// </summary>
    public DateTime StartTime { get; set; }

    /// <summary>
    /// Gets or sets the end time of the show.
    /// </summary>
    public DateTime EndTime { get; set; }

    /// <summary>
    /// Gets or sets the base price for a ticket for this showtime.
    /// </summary>
    public decimal BasePrice { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this showtime is active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the movie associated with this showtime.
    /// </summary>
    public Movie? Movie { get; set; }

    /// <summary>
    /// Gets or sets the hall associated with this showtime.
    /// </summary>
    public Hall? Hall { get; set; }
    
    /// <summary>
    /// Gets or sets the collection of tickets sold for this showtime.
    /// </summary>
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}

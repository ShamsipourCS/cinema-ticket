using CinemaTicket.Domain.Common;

namespace CinemaTicket.Domain.Entities;

/// <summary>
/// Represents a cinema hall or auditorium.
/// </summary>
public class Hall : BaseEntity
{
    /// <summary>
    /// Gets or sets the ID of the cinema this hall belongs to.
    /// </summary>
    public Guid CinemaId { get; set; }

    /// <summary>
    /// Gets or sets the name or number of the hall.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the number of seat rows in the hall.
    /// </summary>
    public int Rows { get; set; }

    /// <summary>
    /// Gets or sets the number of seats in each row.
    /// </summary>
    public int SeatsPerRow { get; set; }

    /// <summary>
    /// Gets or sets the total seating capacity of the hall.
    /// </summary>
    public int TotalCapacity { get; set; }

    /// <summary>
    /// Gets or sets the cinema this hall belongs to.
    /// </summary>
    public Cinema? Cinema { get; set; }

    /// <summary>
    /// Gets or sets the collection of seats in this hall.
    /// </summary>
    public ICollection<Seat> Seats { get; set; } = new List<Seat>();

    /// <summary>
    /// Gets or sets the collection of showtimes scheduled in this hall.
    /// </summary>
    public ICollection<Showtime> Showtimes { get; set; } = new List<Showtime>();
}

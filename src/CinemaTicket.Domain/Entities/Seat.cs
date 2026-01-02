using CinemaTicket.Domain.Common;
using CinemaTicket.Domain.Enums;

namespace CinemaTicket.Domain.Entities;

/// <summary>
/// Represents a specific seat within a cinema hall.
/// </summary>
public class Seat : BaseEntity
{
    /// <summary>
    /// Gets or sets the ID of the hall this seat is in.
    /// </summary>
    public Guid HallId { get; set; }

    /// <summary>
    /// Gets or sets the row identifier (e.g., "A").
    /// </summary>
    public string Row { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the seat number within the row.
    /// </summary>
    public int Number { get; set; }

    /// <summary>
    /// Gets or sets the type of the seat (e.g., Standard, VIP).
    /// </summary>
    public SeatType SeatType { get; set; }

    /// <summary>
    /// Gets or sets the price multiplier for this seat type.
    /// </summary>
    public decimal PriceMultiplier { get; set; } = 1.0m;

    /// <summary>
    /// Gets or sets the hall this seat belongs to.
    /// </summary>
    public Hall? Hall { get; set; }
}

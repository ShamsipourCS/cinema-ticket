using CinemaTicket.Domain.Common;
using CinemaTicket.Domain.Enums;

namespace CinemaTicket.Domain.Entities;

public class Seat : BaseEntity
{
    public Guid HallId { get; set; }
    public string Row { get; set; } = string.Empty; // e.g., "A"
    public int Number { get; set; } // e.g., 1
    public SeatType SeatType { get; set; }
    public decimal PriceMultiplier { get; set; } = 1.0m;

    // Navigation Properties
    public Hall? Hall { get; set; }
}

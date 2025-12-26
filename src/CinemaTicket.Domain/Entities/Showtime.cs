using CinemaTicket.Domain.Common;

namespace CinemaTicket.Domain.Entities;

public class Showtime : BaseEntity
{
    public Guid MovieId { get; set; }
    public Guid HallId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public decimal BasePrice { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation Properties
    public Movie? Movie { get; set; }
    public Hall? Hall { get; set; }
    
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}

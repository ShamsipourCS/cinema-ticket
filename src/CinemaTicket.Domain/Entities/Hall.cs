using CinemaTicket.Domain.Common;

namespace CinemaTicket.Domain.Entities;

public class Hall : BaseEntity
{
    public Guid CinemaId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Rows { get; set; }
    public int SeatsPerRow { get; set; }
    public int TotalCapacity { get; set; }

    // Navigation Properties
    public Cinema? Cinema { get; set; }
    public ICollection<Seat> Seats { get; set; } = new List<Seat>();
    // public ICollection<Showtime> Showtimes { get; set; } = new List<Showtime>();
}

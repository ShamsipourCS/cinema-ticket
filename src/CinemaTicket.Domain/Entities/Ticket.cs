using CinemaTicket.Domain.Common;
using CinemaTicket.Domain.Enums;

namespace CinemaTicket.Domain.Entities;

public class Ticket : AuditableEntity
{
    public Guid UserId { get; set; }
    public Guid ShowtimeId { get; set; }
    public Guid SeatId { get; set; }
    
    public string TicketNumber { get; set; } = string.Empty; // Unique generated ID
    public string HolderName { get; set; } = string.Empty; // For family bookings
    public decimal Price { get; set; }
    public TicketStatus Status { get; set; } = TicketStatus.Pending;

    // Navigation Properties
    public User? User { get; set; }
    public Showtime? Showtime { get; set; }
    public Seat? Seat { get; set; }
    public Payment? Payment { get; set; }
}

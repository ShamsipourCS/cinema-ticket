using CinemaTicket.Domain.Common;
using CinemaTicket.Domain.Enums;

namespace CinemaTicket.Domain.Entities;

/// <summary>
/// Represents a ticket purchased by a user for a specific showtime and seat.
/// </summary>
public class Ticket : AuditableEntity
{
    /// <summary>
    /// Gets or sets the ID of the user who purchased the ticket.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the ID of the showtime for this ticket.
    /// </summary>
    public Guid ShowtimeId { get; set; }

    /// <summary>
    /// Gets or sets the ID of the seat for this ticket.
    /// </summary>
    public Guid SeatId { get; set; }
    
    /// <summary>
    /// Gets or sets the unique ticket number.
    /// </summary>
    public string TicketNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the ticket holder.
    /// </summary>
    public string HolderName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the final price paid for the ticket.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Gets or sets the current status of the ticket (e.g., Pending, Confirmed, Cancelled).
    /// </summary>
    public TicketStatus Status { get; set; } = TicketStatus.Pending;

    /// <summary>
    /// Gets or sets the user who purchased the ticket.
    /// </summary>
    public User? User { get; set; }

    /// <summary>
    /// Gets or sets the showtime associated with this ticket.
    /// </summary>
    public Showtime? Showtime { get; set; }

    /// <summary>
    /// Gets or sets the seat associated with this ticket.
    /// </summary>
    public Seat? Seat { get; set; }

    /// <summary>
    /// Gets or sets the payment details for this ticket.
    /// </summary>
    public Payment? Payment { get; set; }
}

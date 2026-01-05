namespace CinemaTicket.Domain.Enums;

/// <summary>
/// Defines the possible statuses for a ticket.
/// </summary>
public enum TicketStatus
{
    /// <summary>
    /// The ticket has been requested but not yet paid for.
    /// </summary>
    Pending = 0,

    /// <summary>
    /// The ticket has been paid for and confirmed.
    /// </summary>
    Confirmed = 1,

    /// <summary>
    /// The ticket has been cancelled by the user or system.
    /// </summary>
    Cancelled = 2,

    /// <summary>
    /// The ticket reservation has expired due to lack of payment.
    /// </summary>
    Expired = 3
}

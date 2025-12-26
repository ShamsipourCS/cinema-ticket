namespace CinemaTicket.Domain.Enums;

public enum TicketStatus
{
    Pending = 0,
    Confirmed = 1,
    Cancelled = 2,
    Expired = 3 // For when reservation time runs out
}

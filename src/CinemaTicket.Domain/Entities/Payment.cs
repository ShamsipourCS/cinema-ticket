using CinemaTicket.Domain.Common;
using CinemaTicket.Domain.Enums;

namespace CinemaTicket.Domain.Entities;

public class Payment : AuditableEntity
{
    public Guid TicketId { get; set; }
    public string StripePaymentIntentId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "usd";
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    // Navigation Properties
    public Ticket? Ticket { get; set; }
}

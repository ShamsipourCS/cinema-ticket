using CinemaTicket.Domain.Common;
using CinemaTicket.Domain.Enums;

namespace CinemaTicket.Domain.Entities;

/// <summary>
/// Represents a payment transaction for a ticket.
/// </summary>
public class Payment : AuditableEntity
{
    /// <summary>
    /// Gets or sets the ID of the ticket this payment is for.
    /// Null when payment intent is created, set when booking is confirmed.
    /// </summary>
    public Guid? TicketId { get; set; }

    /// <summary>
    /// Gets or sets the external payment intent ID (e.g., from Stripe).
    /// </summary>
    public string StripePaymentIntentId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the payment amount.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Gets or sets the currency used for the payment (e.g., "usd").
    /// </summary>
    public string Currency { get; set; } = "usd";

    /// <summary>
    /// Gets or sets the current status of the payment.
    /// </summary>
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    /// <summary>
    /// Gets or sets the ticket this payment is associated with.
    /// </summary>
    public Ticket? Ticket { get; set; }
}

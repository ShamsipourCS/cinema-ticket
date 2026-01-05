namespace CinemaTicket.Domain.Enums;

/// <summary>
/// Defines the possible statuses for a payment transaction.
/// </summary>
public enum PaymentStatus
{
    /// <summary>
    /// The payment is pending processing.
    /// </summary>
    Pending = 0,

    /// <summary>
    /// The payment was successfully completed.
    /// </summary>
    Success = 1,

    /// <summary>
    /// The payment failed during processing.
    /// </summary>
    Failed = 2,

    /// <summary>
    /// The payment was refunded.
    /// </summary>
    Refunded = 3
}

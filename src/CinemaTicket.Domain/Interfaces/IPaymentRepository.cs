using System;
using System.Threading;
using System.Threading.Tasks;
using CinemaTicket.Domain.Entities;

namespace CinemaTicket.Domain.Interfaces;

/// <summary>
/// Repository interface for Payment entity operations.
/// </summary>
public interface IPaymentRepository : IRepository<Payment>
{
    /// <summary>
    /// Gets a payment by its Stripe Payment Intent ID asynchronously.
    /// </summary>
    /// <param name="stripePaymentIntentId">The Stripe Payment Intent ID.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The payment if found, otherwise null.</returns>
    Task<Payment?> GetByStripePaymentIntentIdAsync(string stripePaymentIntentId, CancellationToken cancellationToken = default);
}

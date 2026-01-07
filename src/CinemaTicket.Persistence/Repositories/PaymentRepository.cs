using CinemaTicket.Domain.Entities;
using CinemaTicket.Domain.Interfaces;
using CinemaTicket.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace CinemaTicket.Persistence.Repositories;

/// <summary>
/// Implementation of the IPaymentRepository for EF Core.
/// </summary>
public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
{
    /// <summary>
    /// Initializes a new instance of the PaymentRepository class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public PaymentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Payment?> GetByStripePaymentIntentIdAsync(string stripePaymentIntentId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<Payment>()
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.StripePaymentIntentId == stripePaymentIntentId, cancellationToken);
    }
}

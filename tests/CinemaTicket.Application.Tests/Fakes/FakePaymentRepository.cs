using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using CinemaTicket.Domain.Entities;
using CinemaTicket.Domain.Interfaces;

namespace CinemaTicket.Application.Tests.Fakes;

public sealed class FakePaymentRepository : IPaymentRepository
{
    private readonly List<Payment> _store = new();

    public void Seed(params Payment[] payments) => _store.AddRange(payments);

    public Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id cannot be empty", nameof(id));

        return Task.FromResult(_store.FirstOrDefault(p => p.Id == id));
    }

    public Task<IEnumerable<Payment>> GetAllAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IEnumerable<Payment>>(_store.ToList());

    public Task<IEnumerable<Payment>> FindAsync(Expression<Func<Payment, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var compiled = predicate.Compile();
        return Task.FromResult<IEnumerable<Payment>>(_store.Where(compiled).ToList());
    }

    public Task AddAsync(Payment entity, CancellationToken cancellationToken = default)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        _store.Add(entity);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Payment entity, CancellationToken cancellationToken = default)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        var idx = _store.FindIndex(p => p.Id == entity.Id);
        if (idx < 0)
            throw new InvalidOperationException($"Payment with ID {entity.Id} not found");

        _store[idx] = entity;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Payment entity, CancellationToken cancellationToken = default)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        _store.RemoveAll(p => p.Id == entity.Id);
        return Task.CompletedTask;
    }

    public Task<Payment?> GetByStripePaymentIntentIdAsync(string stripePaymentIntentId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(stripePaymentIntentId))
            throw new ArgumentException("StripePaymentIntentId cannot be null or empty", nameof(stripePaymentIntentId));

        return Task.FromResult(_store.FirstOrDefault(p =>
            string.Equals(p.StripePaymentIntentId, stripePaymentIntentId, StringComparison.Ordinal)));
    }
}

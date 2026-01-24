using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using CinemaTicket.Domain.Entities;
using CinemaTicket.Domain.Interfaces;

namespace CinemaTicket.Application.Tests.Fakes;

public sealed class FakeTicketRepository : ITicketRepository
{
    private readonly List<Ticket> _store = new();

    public void Seed(params Ticket[] tickets) => _store.AddRange(tickets);

    public Task<Ticket?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id cannot be empty", nameof(id));

        return Task.FromResult(_store.FirstOrDefault(t => t.Id == id));
    }

    public Task<IEnumerable<Ticket>> GetAllAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IEnumerable<Ticket>>(_store.ToList());

    public Task<IEnumerable<Ticket>> FindAsync(Expression<Func<Ticket, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var compiled = predicate.Compile();
        return Task.FromResult<IEnumerable<Ticket>>(_store.Where(compiled).ToList());
    }

    public Task AddAsync(Ticket entity, CancellationToken cancellationToken = default)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        _store.Add(entity);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Ticket entity, CancellationToken cancellationToken = default)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        var idx = _store.FindIndex(t => t.Id == entity.Id);
        if (idx < 0)
            throw new InvalidOperationException($"Ticket with ID {entity.Id} not found");

        _store[idx] = entity;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Ticket entity, CancellationToken cancellationToken = default)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        _store.RemoveAll(t => t.Id == entity.Id);
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(Guid ticketId)
    {
        if (ticketId == Guid.Empty)
            throw new ArgumentException("Id cannot be empty", nameof(ticketId));

        return Task.FromResult(_store.Any(t => t.Id == ticketId));
    }

    public Task<List<Ticket>> GetByShowtimeAsync(Guid showtimeId, CancellationToken cancellationToken = default)
    {
        if (showtimeId == Guid.Empty)
            throw new ArgumentException("ShowtimeId cannot be empty", nameof(showtimeId));

        var tickets = _store.Where(t => t.ShowtimeId == showtimeId).ToList();
        return Task.FromResult(tickets);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using CinemaTicket.Domain.Interfaces;
using CinemaTicket.Domain.Entities;

namespace CinemaTicket.Application.Tests.Fakes;

public sealed class FakeHallRepository : IHallRepository
{
    private readonly List<Hall> _store = new();

    public Task<Hall?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => Task.FromResult(_store.FirstOrDefault(h => h.Id == id));

    public Task<IEnumerable<Hall>> GetAllAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IEnumerable<Hall>>(_store.ToList());

    public Task<IEnumerable<Hall>> FindAsync(Expression<Func<Hall, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var compiled = predicate.Compile();
        return Task.FromResult<IEnumerable<Hall>>(_store.Where(compiled).ToList());
    }

    public Task AddAsync(Hall entity, CancellationToken cancellationToken = default)
    {
        _store.Add(entity);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Hall entity, CancellationToken cancellationToken = default)
    {
        var idx = _store.FindIndex(h => h.Id == entity.Id);
        if (idx >= 0) _store[idx] = entity;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Hall entity, CancellationToken cancellationToken = default)
    {
        _store.RemoveAll(h => h.Id == entity.Id);
        return Task.CompletedTask;
    }
}

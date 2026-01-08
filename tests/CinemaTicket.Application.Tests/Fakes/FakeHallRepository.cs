using System.Linq.Expressions;
using CinemaTicket.Application.Common.Interfaces;
using CinemaTicket.Domain.Entities;

namespace CinemaTicket.Application.Tests.Fakes;

public sealed class FakeHallRepository : IHallRepository
{
    private readonly List<Hall> _store = new();

    public void Seed(params Hall[] halls) => _store.AddRange(halls);

    public Task<Hall?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => Task.FromResult(_store.FirstOrDefault(h => h.Id == id));

    public Task<IEnumerable<Hall>> GetAllAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IEnumerable<Hall>>(_store.ToList());

    public Task<IEnumerable<Hall>> FindAsync(Expression<Func<Hall, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var compiled = predicate.Compile();
        var result = _store.Where(compiled).ToList();
        return Task.FromResult<IEnumerable<Hall>>(result);
    }

    public Task AddAsync(Hall entity, CancellationToken cancellationToken = default)
    {
        _store.Add(entity);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Hall entity, CancellationToken cancellationToken = default)
    {
        var index = _store.FindIndex(h => h.Id == entity.Id);
        if (index >= 0)
            _store[index] = entity;

        return Task.CompletedTask;
    }

    public Task DeleteAsync(Hall entity, CancellationToken cancellationToken = default)
    {
        _store.RemoveAll(h => h.Id == entity.Id);
        return Task.CompletedTask;
    }
}

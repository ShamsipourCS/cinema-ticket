using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using CinemaTicket.Domain.Interfaces;
using CinemaTicket.Domain.Entities;

namespace CinemaTicket.Application.Tests.Fakes;

public sealed class FakeCinemaRepository : ICinemaRepository
{
    private readonly List<Cinema> _store = new();

    public int AddCalls { get; private set; }
    public int UpdateCalls { get; private set; }
    public int DeleteCalls { get; private set; }

    public void Seed(params Cinema[] cinemas) => _store.AddRange(cinemas);

    public Task<Cinema?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => Task.FromResult(_store.FirstOrDefault(c => c.Id == id));

    public Task<IEnumerable<Cinema>> GetAllAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IEnumerable<Cinema>>(_store.ToList());

    public Task<IEnumerable<Cinema>> FindAsync(Expression<Func<Cinema, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var compiled = predicate.Compile();
        return Task.FromResult<IEnumerable<Cinema>>(_store.Where(compiled).ToList());
    }

    public Task AddAsync(Cinema entity, CancellationToken cancellationToken = default)
    {
        AddCalls++;
        _store.Add(entity);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Cinema entity, CancellationToken cancellationToken = default)
    {
        UpdateCalls++;

        var idx = _store.FindIndex(c => c.Id == entity.Id);
        if (idx >= 0)
        {
            _store[idx] = entity;
        }
        else
        {
            _store.Add(entity);
        }

        return Task.CompletedTask;
    }

    public Task DeleteAsync(Cinema entity, CancellationToken cancellationToken = default)
    {
        DeleteCalls++;
        _store.RemoveAll(c => c.Id == entity.Id);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<Cinema>> GetActiveCinemasAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IEnumerable<Cinema>>(_store.Where(c => c.IsActive).ToList());

    public Task<IEnumerable<Cinema>> GetByCityAsync(string city, CancellationToken cancellationToken = default)
        => Task.FromResult<IEnumerable<Cinema>>(
            _store.Where(c => string.Equals(c.City, city, StringComparison.OrdinalIgnoreCase)).ToList()
        );

    public Task<bool> ExistsByNameInCityAsync(string name, string city, CancellationToken cancellationToken = default)
        => Task.FromResult(_store.Any(c =>
            string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(c.City, city, StringComparison.OrdinalIgnoreCase)));
}

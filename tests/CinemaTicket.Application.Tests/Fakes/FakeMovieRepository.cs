using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using CinemaTicket.Domain.Interfaces;
using CinemaTicket.Domain.Entities;

namespace CinemaTicket.Application.Tests.Fakes;

public sealed class FakeMovieRepository : IMovieRepository
{
    private readonly List<Movie> _store = new();

    public int AddCalls { get; private set; }
    public int UpdateCalls { get; private set; }
    public int DeleteCalls { get; private set; }

    public void Seed(params Movie[] movies) => _store.AddRange(movies);

    public Task<Movie?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => Task.FromResult(_store.FirstOrDefault(m => m.Id == id));

    public Task<IEnumerable<Movie>> GetAllAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IEnumerable<Movie>>(_store.ToList());

    public Task<IEnumerable<Movie>> FindAsync(Expression<Func<Movie, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var compiled = predicate.Compile();
        return Task.FromResult<IEnumerable<Movie>>(_store.Where(compiled).ToList());
    }

    public Task AddAsync(Movie entity, CancellationToken cancellationToken = default)
    {
        AddCalls++;
        _store.Add(entity);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Movie entity, CancellationToken cancellationToken = default)
    {
        UpdateCalls++;
        var idx = _store.FindIndex(m => m.Id == entity.Id);
        if (idx >= 0) _store[idx] = entity;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Movie entity, CancellationToken cancellationToken = default)
    {
        DeleteCalls++;
        _store.RemoveAll(m => m.Id == entity.Id);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<Movie>> GetActiveMoviesAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IEnumerable<Movie>>(_store.Where(m => m.IsActive).ToList());

    public Task<IEnumerable<Movie>> GetByGenreAsync(string genre, CancellationToken cancellationToken = default)
        => Task.FromResult<IEnumerable<Movie>>(_store.Where(m => m.Genre == genre).ToList());

    public Task<bool> ExistsWithTitleAsync(string title, CancellationToken cancellationToken = default)
        => Task.FromResult(_store.Any(m => string.Equals(m.Title, title, StringComparison.OrdinalIgnoreCase)));
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using CinemaTicket.Domain.Entities;
using CinemaTicket.Domain.Interfaces;

namespace CinemaTicket.Application.Tests.Fakes;

public sealed class FakeUserRepository : IUserRepository
{
    private readonly List<User> _store = new();

    public void Seed(params User[] users) => _store.AddRange(users);

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id cannot be empty", nameof(id));

        return Task.FromResult(_store.FirstOrDefault(u => u.Id == id));
    }

    public Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IEnumerable<User>>(_store.ToList());

    public Task<IEnumerable<User>> FindAsync(Expression<Func<User, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var compiled = predicate.Compile();
        return Task.FromResult<IEnumerable<User>>(_store.Where(compiled).ToList());
    }

    public Task AddAsync(User entity, CancellationToken cancellationToken = default)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        _store.Add(entity);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(User entity, CancellationToken cancellationToken = default)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        var idx = _store.FindIndex(u => u.Id == entity.Id);
        if (idx < 0)
            throw new InvalidOperationException($"User with ID {entity.Id} not found");

        _store[idx] = entity;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(User entity, CancellationToken cancellationToken = default)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        _store.RemoveAll(u => u.Id == entity.Id);
        return Task.CompletedTask;
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be null or empty", nameof(email));

        return Task.FromResult(_store.FirstOrDefault(u =>
            string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase)));
    }

    public Task<User?> GetByEmailWithRefreshTokensAsync(string email, CancellationToken cancellationToken = default)
        => GetByEmailAsync(email, cancellationToken);
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using CinemaTicket.Domain.Entities;
using CinemaTicket.Domain.Interfaces;

namespace CinemaTicket.Application.Tests.Fakes;

public sealed class FakeRefreshTokenRepository : IRefreshTokenRepository
{
    private readonly List<RefreshToken> _store = new();

    public void Seed(params RefreshToken[] tokens) => _store.AddRange(tokens);

    public Task<RefreshToken?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id cannot be empty", nameof(id));

        return Task.FromResult(_store.FirstOrDefault(t => t.Id == id));
    }

    public Task<IEnumerable<RefreshToken>> GetAllAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IEnumerable<RefreshToken>>(_store.ToList());

    public Task<IEnumerable<RefreshToken>> FindAsync(Expression<Func<RefreshToken, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var compiled = predicate.Compile();
        return Task.FromResult<IEnumerable<RefreshToken>>(_store.Where(compiled).ToList());
    }

    public Task AddAsync(RefreshToken entity, CancellationToken cancellationToken = default)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        _store.Add(entity);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(RefreshToken entity, CancellationToken cancellationToken = default)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        var idx = _store.FindIndex(t => t.Id == entity.Id);
        if (idx < 0)
            throw new InvalidOperationException($"RefreshToken with ID {entity.Id} not found");

        _store[idx] = entity;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(RefreshToken entity, CancellationToken cancellationToken = default)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        _store.RemoveAll(t => t.Id == entity.Id);
        return Task.CompletedTask;
    }

    public Task<RefreshToken?> GetValidTokenAsync(string token, Guid userId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token cannot be null or empty", nameof(token));
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty", nameof(userId));

        var now = DateTime.UtcNow;
        var result = _store.FirstOrDefault(t =>
            string.Equals(t.Token, token, StringComparison.Ordinal) &&
            t.UserId == userId &&
            !t.IsRevoked &&
            t.ExpiresAt > now);
        return Task.FromResult(result);
    }

    public Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token cannot be null or empty", nameof(token));

        return Task.FromResult(_store.FirstOrDefault(t => string.Equals(t.Token, token, StringComparison.Ordinal)));
    }

    public Task RevokeAllUserTokensAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty", nameof(userId));

        foreach (var token in _store.Where(t => t.UserId == userId))
        {
            token.IsRevoked = true;
        }

        return Task.CompletedTask;
    }

    public Task<int> RemoveExpiredTokensAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var removed = _store.RemoveAll(t => t.ExpiresAt <= now);
        return Task.FromResult(removed);
    }

    public Task<IEnumerable<RefreshToken>> GetActiveTokensForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty", nameof(userId));

        var now = DateTime.UtcNow;
        var result = _store.Where(t => t.UserId == userId && !t.IsRevoked && t.ExpiresAt > now).ToList();
        return Task.FromResult<IEnumerable<RefreshToken>>(result);
    }
}

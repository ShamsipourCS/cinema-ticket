using System;
using System.Threading;
using System.Threading.Tasks;
using CinemaTicket.Domain.Interfaces;

namespace CinemaTicket.Application.Tests.Fakes;

public sealed class FakeUnitOfWork : IUnitOfWork
{
    public FakeUnitOfWork(
        IMovieRepository movies,
        ICinemaRepository cinemas,
        IHallRepository halls)
    {
        Movies = movies;
        Cinemas = cinemas;
        Halls = halls;
    }

    public IUserRepository Users => throw new NotImplementedException("Users repository not needed for catalog handler tests.");
    public IRefreshTokenRepository RefreshTokens => throw new NotImplementedException("RefreshTokens repository not needed for catalog handler tests.");

    public IMovieRepository Movies { get; }
    public ICinemaRepository Cinemas { get; }
    public IHallRepository Halls { get; }

    public int SaveChangesCalls { get; private set; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SaveChangesCalls++;
        return Task.FromResult(1);
    }

    public Task BeginTransactionAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task CommitTransactionAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task RollbackTransactionAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public void Dispose()
    {
        // Nothing to dispose in fake
    }
}

using System;
using System.Threading;
using System.Threading.Tasks;
using CinemaTicket.Domain.Interfaces;

namespace CinemaTicket.Application.Tests.Fakes;

public sealed class FakeUnitOfWork : IUnitOfWork
{
    public FakeUnitOfWork(
        IUserRepository users,
        IRefreshTokenRepository refreshTokens,
        IPaymentRepository payments,
        ITicketRepository tickets,
        IMovieRepository movies,
        ICinemaRepository cinemas,
        IHallRepository halls)
    {
        Users = users;
        RefreshTokens = refreshTokens;
        Payments = payments;
        Tickets = tickets;
        Movies = movies;
        Cinemas = cinemas;
        Halls = halls;
    }

    // Backward compatibility constructor for existing tests (old 6-parameter signature)
    public FakeUnitOfWork(
        IUserRepository users,
        IRefreshTokenRepository refreshTokens,
        IPaymentRepository payments,
        IMovieRepository movies,
        ICinemaRepository cinemas,
        IHallRepository halls)
        : this(
            users,
            refreshTokens,
            payments,
            new FakeTicketRepository(),  // Add default Tickets repository
            movies,
            cinemas,
            halls)
    {
    }

    public FakeUnitOfWork(
        IMovieRepository movies,
        ICinemaRepository cinemas,
        IHallRepository halls)
        : this(
            new FakeUserRepository(),
            new FakeRefreshTokenRepository(),
            new FakePaymentRepository(),
            new FakeTicketRepository(),
            movies,
            cinemas,
            halls)
    {
    }

    public FakeUnitOfWork()
        : this(
            new FakeUserRepository(),
            new FakeRefreshTokenRepository(),
            new FakePaymentRepository(),
            new FakeTicketRepository(),
            new FakeMovieRepository(),
            new FakeCinemaRepository(),
            new FakeHallRepository())
    {
    }

    public IUserRepository Users { get; }
    public IRefreshTokenRepository RefreshTokens { get; }
    public IMovieRepository Movies { get; }
    public ICinemaRepository Cinemas { get; }
    public IHallRepository Halls { get; }
    public ITicketRepository Tickets { get; }
    public IPaymentRepository Payments { get; }

    public int SaveChangesCalls { get; private set; }
    public int BeginTransactionCalls { get; private set; }
    public int CommitTransactionCalls { get; private set; }
    public int RollbackTransactionCalls { get; private set; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SaveChangesCalls++;
        return Task.FromResult(1);
    }

    public Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        BeginTransactionCalls++;
        return Task.CompletedTask;
    }

    public Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        CommitTransactionCalls++;
        return Task.CompletedTask;
    }

    public Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        RollbackTransactionCalls++;
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        // Nothing to dispose in fake
    }
}

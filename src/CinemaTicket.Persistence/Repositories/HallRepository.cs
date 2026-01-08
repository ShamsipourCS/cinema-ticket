using System.Linq.Expressions;
using CinemaTicket.Domain.Interfaces;
using CinemaTicket.Domain.Entities;
using CinemaTicket.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace CinemaTicket.Persistence.Repositories;

public sealed class HallRepository : IHallRepository
{
    private readonly ApplicationDbContext _context;

    public HallRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Hall?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Halls.FirstOrDefaultAsync(h => h.Id == id, cancellationToken);

    public async Task<IEnumerable<Hall>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Halls.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<IEnumerable<Hall>> FindAsync(Expression<Func<Hall, bool>> predicate, CancellationToken cancellationToken = default)
        => await _context.Halls.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);

    public async Task AddAsync(Hall entity, CancellationToken cancellationToken = default)
        => await _context.Halls.AddAsync(entity, cancellationToken);

    public Task UpdateAsync(Hall entity, CancellationToken cancellationToken = default)
    {
        _context.Halls.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Hall entity, CancellationToken cancellationToken = default)
    {
        _context.Halls.Remove(entity);
        return Task.CompletedTask;
    }
}

using System.Linq.Expressions;
using CinemaTicket.Domain.Entities;
using CinemaTicket.Domain.Interfaces;
using CinemaTicket.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace CinemaTicket.Persistence.Repositories;

public class CinemaRepository : ICinemaRepository
{
    private readonly ApplicationDbContext _context;

    public CinemaRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Cinema?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Cinemas.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task<IEnumerable<Cinema>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Cinemas.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<IEnumerable<Cinema>> FindAsync(Expression<Func<Cinema, bool>> predicate, CancellationToken cancellationToken = default)
        => await _context.Cinemas.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);

    public async Task AddAsync(Cinema entity, CancellationToken cancellationToken = default)
        => await _context.Cinemas.AddAsync(entity, cancellationToken);

    public Task UpdateAsync(Cinema entity, CancellationToken cancellationToken = default)
    {
        _context.Cinemas.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Cinema entity, CancellationToken cancellationToken = default)
    {
        _context.Cinemas.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<Cinema>> GetActiveCinemasAsync(CancellationToken cancellationToken = default)
        => await _context.Cinemas.AsNoTracking().Where(c => c.IsActive).ToListAsync(cancellationToken);

    public async Task<IEnumerable<Cinema>> GetByCityAsync(string city, CancellationToken cancellationToken = default)
        => await _context.Cinemas.AsNoTracking().Where(c => c.City == city).ToListAsync(cancellationToken);

    public async Task<bool> ExistsByNameInCityAsync(string name, string city, CancellationToken cancellationToken = default)
        => await _context.Cinemas.AnyAsync(c => c.Name == name && c.City == city, cancellationToken);
}

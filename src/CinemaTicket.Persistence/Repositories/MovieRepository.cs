using System.Linq.Expressions;
using CinemaTicket.Domain.Entities;
using CinemaTicket.Domain.Interfaces;
using CinemaTicket.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace CinemaTicket.Persistence.Repositories;

public class MovieRepository : IMovieRepository
{
    private readonly ApplicationDbContext _context;

    public MovieRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Movie?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Movies.FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

    public async Task<IEnumerable<Movie>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Movies.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<IEnumerable<Movie>> FindAsync(Expression<Func<Movie, bool>> predicate, CancellationToken cancellationToken = default)
        => await _context.Movies.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);

    public async Task AddAsync(Movie entity, CancellationToken cancellationToken = default)
        => await _context.Movies.AddAsync(entity, cancellationToken);

    public Task UpdateAsync(Movie entity, CancellationToken cancellationToken = default)
    {
        _context.Movies.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Movie entity, CancellationToken cancellationToken = default)
    {
        _context.Movies.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<IEnumerable<Movie>> GetActiveMoviesAsync(CancellationToken cancellationToken = default)
        => await _context.Movies.AsNoTracking().Where(m => m.IsActive).ToListAsync(cancellationToken);

    public async Task<IEnumerable<Movie>> GetByGenreAsync(string genre, CancellationToken cancellationToken = default)
        => await _context.Movies.AsNoTracking().Where(m => m.Genre == genre).ToListAsync(cancellationToken);

    public async Task<bool> ExistsWithTitleAsync(string title, CancellationToken cancellationToken = default)
        => await _context.Movies.AnyAsync(m => m.Title == title, cancellationToken);
}

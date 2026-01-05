using CinemaTicket.Application.Common.Interfaces;
using CinemaTicket.Domain.Entities;
using CinemaTicket.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace CinemaTicket.Infrastructure.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        private readonly ApplicationDbContext _context;

        public MovieRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Movie?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Movies
                .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<Movie>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Movies
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Movie movie, CancellationToken cancellationToken = default)
        {
            await _context.Movies.AddAsync(movie, cancellationToken);
        }

        public void Update(Movie movie)
        {
            _context.Movies.Update(movie);
        }

        public void Delete(Movie movie)
        {
            _context.Movies.Remove(movie);
        }
    }
}

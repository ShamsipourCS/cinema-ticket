using CinemaTicket.Application.Common.Interfaces;
using CinemaTicket.Domain.Entities;
using CinemaTicket.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace CinemaTicket.Infrastructure.Repositories
{
    public class CinemaRepository : ICinemaRepository
    {
        private readonly ApplicationDbContext _context;

        public CinemaRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Cinema?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Cinemas
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<Cinema>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Cinemas
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Cinema cinema, CancellationToken cancellationToken = default)
        {
            await _context.Cinemas.AddAsync(cinema, cancellationToken);
        }

        public void Update(Cinema cinema)
        {
            _context.Cinemas.Update(cinema);
        }

        public void Delete(Cinema cinema)
        {
            _context.Cinemas.Remove(cinema);
        }
    }
}

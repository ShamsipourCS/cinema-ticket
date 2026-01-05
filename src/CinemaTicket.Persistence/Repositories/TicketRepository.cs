using CinemaTicket.Application.Common.Interfaces;
using CinemaTicket.Domain.Entities;
using CinemaTicket.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CinemaTicket.Persistence.Repositories
{
    public class TicketRepository : GenericRepository<Ticket>, ITicketRepository
    {
        private readonly ApplicationDbContext _context;

        public TicketRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        // متد برای بررسی وجود تیکت
        public async Task<bool> ExistsAsync(Guid ticketId)
        {
            return await _context.Set<Ticket>().AnyAsync(t => t.Id == ticketId);
        }

        // متد برای گرفتن تیکت‌ها بر اساس Showtime
        public async Task<List<Ticket>> GetByShowtimeAsync(Guid showtimeId, CancellationToken cancellationToken)
        {
            return await _context.Set<Ticket>()
                .Where(t => t.ShowtimeId == showtimeId)
                .ToListAsync(cancellationToken);
        }

        // پیاده‌سازی سایر متدها از IRepository<T>
        public async Task<Ticket> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Set<Ticket>()
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        }

        public async Task<List<Ticket>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.Set<Ticket>().ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Ticket entity, CancellationToken cancellationToken)
        {
            await _context.Set<Ticket>().AddAsync(entity, cancellationToken);
        }

        public async Task UpdateAsync(Ticket entity, CancellationToken cancellationToken)
        {
            _context.Set<Ticket>().Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(Ticket entity, CancellationToken cancellationToken)
        {
            _context.Set<Ticket>().Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<Ticket>> FindAsync(Expression<Func<Ticket, bool>> predicate, CancellationToken cancellationToken)
        {
            return await _context.Set<Ticket>()
                .Where(predicate)
                .ToListAsync(cancellationToken);
        }
    }
}

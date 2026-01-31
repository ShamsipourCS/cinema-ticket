using CinemaTicket.Domain.Entities;
using CinemaTicket.Domain.Interfaces;
using CinemaTicket.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace CinemaTicket.Persistence.Repositories
{
    public class TicketRepository : GenericRepository<Ticket>, ITicketRepository
    {
        public TicketRepository(ApplicationDbContext context) : base(context) { }

        public async Task<bool> ExistsAsync(Guid ticketId)
        {
            return await _context.Set<Ticket>().AnyAsync(t => t.Id == ticketId);
        }

        public async Task<List<Ticket>> GetByShowtimeAsync(Guid showtimeId, CancellationToken cancellationToken)
        {
            return await _context.Set<Ticket>()
                .AsNoTracking()
                .Where(t => t.ShowtimeId == showtimeId)
                .ToListAsync(cancellationToken);
        }
    }
}

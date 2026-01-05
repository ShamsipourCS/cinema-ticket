using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using CinemaTicket.Domain.Entities;

namespace CinemaTicket.Application.Common.Interfaces
{
    public interface ITicketRepository : IRepository<Ticket>
    {
        Task<bool> ExistsAsync(Guid ticketId);

        Task<List<Ticket>> GetByShowtimeAsync(Guid showtimeId, CancellationToken cancellationToken);
    }
}

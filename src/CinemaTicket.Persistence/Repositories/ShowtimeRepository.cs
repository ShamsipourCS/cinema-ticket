using CinemaTicket.Domain.Entities;
using CinemaTicket.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace CinemaTicket.Persistence.Repositories
{
    public class ShowtimeRepository : GenericRepository<Showtime>, IShowtimeRepository
    {
        public ShowtimeRepository(ApplicationDbContext context) : base(context)
        {
        }

        
    }
}

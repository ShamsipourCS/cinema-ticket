using CinemaTicket.Domain.Entities;

namespace CinemaTicket.Domain.Interfaces;

public interface ICinemaRepository : IRepository<Cinema>
{
    Task<IEnumerable<Cinema>> GetActiveCinemasAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Cinema>> GetByCityAsync(string city, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameInCityAsync(string name, string city, CancellationToken cancellationToken = default);
}

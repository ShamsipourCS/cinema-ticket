using CinemaTicket.Domain.Entities;

namespace CinemaTicket.Application.Common.Interfaces
{
    public interface ICinemaRepository
    {
        Task<Cinema?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Cinema>> GetAllAsync(CancellationToken cancellationToken = default);
        Task AddAsync(Cinema cinema, CancellationToken cancellationToken = default);
        void Update(Cinema cinema);
        void Delete(Cinema cinema);
    }
}

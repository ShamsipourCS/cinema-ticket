using CinemaTicket.Domain.Entities;

namespace CinemaTicket.Application.Common.Interfaces
{
    public interface IMovieRepository
    {
        Task<Movie?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Movie>> GetAllAsync(CancellationToken cancellationToken = default);
        Task AddAsync(Movie movie, CancellationToken cancellationToken = default);
        void Update(Movie movie);
        void Delete(Movie movie);
    }
}

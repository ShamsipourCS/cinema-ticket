using CinemaTicket.Domain.Entities;

namespace CinemaTicket.Application.Common.Interfaces;

public interface IMovieRepository : IRepository<Movie>
{
    Task<IEnumerable<Movie>> GetActiveMoviesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Movie>> GetByGenreAsync(string genre, CancellationToken cancellationToken = default);
    Task<bool> ExistsWithTitleAsync(string title, CancellationToken cancellationToken = default);
}

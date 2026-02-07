using MediatR;
using Microsoft.EntityFrameworkCore;
using CinemaTicket.Application.Common.Interfaces;

namespace CinemaTicket.Application.Features.Showtimes.Commands.DeleteShowtime;

public sealed class DeleteShowtimeCommandHandler : IRequestHandler<DeleteShowtimeCommand>
{
    private readonly IApplicationDbContext _db;

    public DeleteShowtimeCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task Handle(DeleteShowtimeCommand request, CancellationToken cancellationToken)
    {
        var entity = await _db.Showtimes
            .FirstOrDefaultAsync(s => s.Id == request.ShowtimeId, cancellationToken);

        if (entity is null)
            throw new KeyNotFoundException("Showtime not found.");

        _db.Showtimes.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
    }
}

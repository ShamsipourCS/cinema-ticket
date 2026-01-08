using MediatR;
using Microsoft.EntityFrameworkCore;
using CinemaTicket.Application.Common.Interfaces;
using CinemaTicket.Application.Features.Showtimes.DTOs;

namespace CinemaTicket.Application.Features.Showtimes.Queries.GetShowtimeById;

public sealed class GetShowtimeByIdQueryHandler : IRequestHandler<GetShowtimeByIdQuery, ShowtimeDto>
{
    private readonly IApplicationDbContext _db;

    public GetShowtimeByIdQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<ShowtimeDto> Handle(GetShowtimeByIdQuery request, CancellationToken cancellationToken)
    {
        var s = await _db.Showtimes.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.ShowtimeId, cancellationToken);

        if (s is null)
            throw new KeyNotFoundException("Showtime not found.");

        return new ShowtimeDto(s.Id, s.MovieId, s.HallId, s.StartTime, s.EndTime, s.BasePrice, s.IsActive);
    }
}

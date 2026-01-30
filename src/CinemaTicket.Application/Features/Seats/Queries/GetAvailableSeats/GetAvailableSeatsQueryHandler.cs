using MediatR;
using Microsoft.EntityFrameworkCore;
using CinemaTicket.Application.Common.Interfaces;
using CinemaTicket.Application.Features.Seats.DTOs;
using CinemaTicket.Domain.Enums;

namespace CinemaTicket.Application.Features.Seats.Queries.GetAvailableSeats;

public sealed class GetAvailableSeatsQueryHandler : IRequestHandler<GetAvailableSeatsQuery, IReadOnlyList<SeatAvailabilityDto>>
{
    private readonly IApplicationDbContext _db;

    public GetAvailableSeatsQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<IReadOnlyList<SeatAvailabilityDto>> Handle(GetAvailableSeatsQuery request, CancellationToken cancellationToken)
    {
        var showtime = await _db.Showtimes.AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.ShowtimeId, cancellationToken);

        if (showtime is null)
            throw new KeyNotFoundException("Showtime not found.");

        if (!showtime.IsActive)
            throw new InvalidOperationException("Showtime is not active.");

        var seats = await _db.Seats.AsNoTracking()
            .Where(s => s.HallId == showtime.HallId)
            .Select(s => new { s.Id, s.Row, s.Number })
            .ToListAsync(cancellationToken);

        var takenSeatIds = await _db.Tickets.AsNoTracking()
            .Where(t => t.ShowtimeId == showtime.Id
                    && t.Status != TicketStatus.Cancelled
                    && t.Status != TicketStatus.Expired)
            .Select(t => t.SeatId)
            .Distinct()
            .ToListAsync(cancellationToken);

        var taken = takenSeatIds.ToHashSet();

        return seats
            .OrderBy(s => s.Row)
            .ThenBy(s => s.Number)
            .Select(s => new SeatAvailabilityDto(
                SeatId: s.Id,
                Label: $"{s.Row}-{s.Number}",
                IsAvailable: !taken.Contains(s.Id)
            ))
            .ToList();
    }
}

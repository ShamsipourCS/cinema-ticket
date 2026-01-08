using MediatR;
using Microsoft.EntityFrameworkCore;
using CinemaTicket.Application.Common.Interfaces;
using CinemaTicket.Application.Features.Showtimes.DTOs;
using CinemaTicket.Domain.Entities;

namespace CinemaTicket.Application.Features.Showtimes.Commands.CreateShowtime;

public sealed class CreateShowtimeCommandHandler : IRequestHandler<CreateShowtimeCommand, ShowtimeDto>
{
    private readonly IApplicationDbContext _db;

    public CreateShowtimeCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<ShowtimeDto> Handle(CreateShowtimeCommand request, CancellationToken cancellationToken)
    {
        var hasOverlap = await _db.Showtimes.AnyAsync(s =>
            s.HallId == request.HallId &&
            s.StartTime < request.EndTime &&
            request.StartTime < s.EndTime,
            cancellationToken);

        if (hasOverlap)
            throw new InvalidOperationException("Showtime overlaps with an existing showtime in this hall.");

        var showtime = new Showtime
        {
            Id = Guid.NewGuid(),
            MovieId = request.MovieId,
            HallId = request.HallId,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            BasePrice = request.BasePrice,
            IsActive = true
        };

        _db.Showtimes.Add(showtime);
        await _db.SaveChangesAsync(cancellationToken);

        return new ShowtimeDto(
            showtime.Id,
            showtime.MovieId,
            showtime.HallId,
            showtime.StartTime,
            showtime.EndTime,
            showtime.BasePrice,
            showtime.IsActive
        );
    }
}

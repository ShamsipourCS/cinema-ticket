using MediatR;
using Microsoft.EntityFrameworkCore;
using CinemaTicket.Application.Features.Showtimes.DTOs;
using CinemaTicket.Application.Common.Interfaces;
using CinemaTicket.Domain.Entities;

namespace CinemaTicket.Application.Features.Showtimes.Commands.UpdateShowtime;

public sealed class UpdateShowtimeCommandHandler
    : IRequestHandler<UpdateShowtimeCommand, ShowtimeDto>
{
    private readonly IApplicationDbContext _context;

    public UpdateShowtimeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ShowtimeDto> Handle(UpdateShowtimeCommand request, CancellationToken cancellationToken)
    {
        var showtime = await _context.Showtimes
            .FirstOrDefaultAsync(s => s.Id == request.ShowtimeId, cancellationToken);

        if (showtime is null)
            throw new KeyNotFoundException($"Showtime with id '{request.ShowtimeId}' not found.");

        var hasOverlap = await _context.Showtimes
            .AsNoTracking()
            .AnyAsync(s =>
                s.Id != request.ShowtimeId &&
                s.HallId == request.HallId &&
                s.StartTime < request.EndsAt &&
                request.StartsAt < s.EndTime,
                cancellationToken);

        if (hasOverlap)
            throw new InvalidOperationException("Showtime overlaps with an existing showtime in the same hall.");

        showtime.MovieId = request.MovieId;
        showtime.HallId = request.HallId;
        showtime.StartTime = request.StartsAt;
        showtime.EndTime = request.EndsAt;
        showtime.IsActive = request.IsActive;
        showtime.BasePrice = request.Price;

        await _context.SaveChangesAsync(cancellationToken);

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

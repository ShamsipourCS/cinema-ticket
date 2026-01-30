using CinemaTicket.Application.Common.Interfaces;
using CinemaTicket.Application.Features.Showtimes.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CinemaTicket.Application.Features.Showtimes.Queries.GetShowtimes;

public sealed class GetShowtimesQueryHandler : IRequestHandler<GetShowtimesQuery, IReadOnlyList<ShowtimeDto>>
{
    private readonly IApplicationDbContext _context;

    public GetShowtimesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<ShowtimeDto>> Handle(GetShowtimesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Showtimes.AsNoTracking().AsQueryable();

        if (request.MovieId.HasValue)
            query = query.Where(s => s.MovieId == request.MovieId.Value);

        if (request.HallId.HasValue)
            query = query.Where(s => s.HallId == request.HallId.Value);

        if (request.From.HasValue)
            query = query.Where(s => s.StartTime >= request.From.Value);

        if (request.To.HasValue)
            query = query.Where(s => s.StartTime <= request.To.Value);

        if (request.IsActive.HasValue)
            query = query.Where(s => s.IsActive == request.IsActive.Value);

        var result = await query
            .OrderBy(s => s.StartTime)
            .Select(s => new ShowtimeDto(
                s.Id,
                s.MovieId,
                s.HallId,
                s.StartTime,
                s.EndTime,
                s.BasePrice,
                s.IsActive
            ))
            .ToListAsync(cancellationToken);

        return result;
    }
}

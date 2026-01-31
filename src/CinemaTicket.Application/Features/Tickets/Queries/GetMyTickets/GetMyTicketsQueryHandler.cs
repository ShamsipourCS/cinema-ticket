using MediatR;
using Microsoft.EntityFrameworkCore;
using CinemaTicket.Application.Common.Interfaces;
using CinemaTicket.Application.Features.Tickets.DTOs;

namespace CinemaTicket.Application.Features.Tickets.Queries.GetMyTickets;

public sealed class GetMyTicketsQueryHandler : IRequestHandler<GetMyTicketsQuery, IReadOnlyList<TicketDto>>
{
    private readonly IApplicationDbContext _db;

    public GetMyTicketsQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<IReadOnlyList<TicketDto>> Handle(GetMyTicketsQuery request, CancellationToken ct)
    {
        return await _db.Tickets.AsNoTracking()
            .Where(t => t.UserId == request.UserId)
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new TicketDto(
                t.Id,
                t.TicketNumber,
                t.ShowtimeId,
                t.SeatId,
                t.Price,
                t.Status,
                t.CreatedAt
            ))
            .ToListAsync(ct);
    }
}

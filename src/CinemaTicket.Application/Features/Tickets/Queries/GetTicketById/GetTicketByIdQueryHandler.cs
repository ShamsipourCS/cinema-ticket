using MediatR;
using Microsoft.EntityFrameworkCore;
using CinemaTicket.Application.Common.Interfaces;
using CinemaTicket.Application.Features.Tickets.DTOs;

namespace CinemaTicket.Application.Features.Tickets.Queries.GetTicketById;

public sealed class GetTicketByIdQueryHandler : IRequestHandler<GetTicketByIdQuery, TicketDto>
{
    private readonly IApplicationDbContext _db;
    public GetTicketByIdQueryHandler(IApplicationDbContext db) => _db = db;

    public async Task<TicketDto> Handle(GetTicketByIdQuery request, CancellationToken ct)
    {
        var ticket = await _db.Tickets.AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == request.TicketId && t.UserId == request.UserId, ct);

        if (ticket is null)
            throw new KeyNotFoundException("Ticket not found.");

        return new TicketDto(
            ticket.Id,
            ticket.TicketNumber,
            ticket.ShowtimeId,
            ticket.SeatId,
            ticket.Price,
            ticket.Status,
            ticket.CreatedAt
        );
    }
}

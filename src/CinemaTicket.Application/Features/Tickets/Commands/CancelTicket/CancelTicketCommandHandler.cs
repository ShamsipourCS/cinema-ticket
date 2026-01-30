using MediatR;
using Microsoft.EntityFrameworkCore;
using CinemaTicket.Application.Common.Interfaces;
using CinemaTicket.Domain.Enums;

namespace CinemaTicket.Application.Features.Tickets.Commands.CancelTicket;

public sealed class CancelTicketCommandHandler : IRequestHandler<CancelTicketCommand>
{
    private readonly IApplicationDbContext _db;
    public CancelTicketCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task Handle(CancelTicketCommand request, CancellationToken ct)
    {
        var ticket = await _db.Tickets
            .FirstOrDefaultAsync(t => t.Id == request.TicketId && t.UserId == request.UserId, ct);

        if (ticket is null)
            throw new KeyNotFoundException("Ticket not found.");

        // business rule: only Pending can be canceled (نمونه)
        if (ticket.Status != TicketStatus.Pending)
            throw new InvalidOperationException("Only pending tickets can be cancelled.");

        ticket.Status = TicketStatus.Cancelled;
        await _db.SaveChangesAsync(ct);
    }
}

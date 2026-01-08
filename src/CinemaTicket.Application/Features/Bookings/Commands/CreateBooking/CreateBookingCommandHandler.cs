using System.Transactions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using CinemaTicket.Application.Common.Interfaces;
using CinemaTicket.Application.Features.Bookings.DTOs;
using CinemaTicket.Domain.Entities;
using CinemaTicket.Domain.Enums;

namespace CinemaTicket.Application.Features.Bookings.Commands.CreateBooking;

public sealed class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, BookingResultDto>
{
    private readonly IApplicationDbContext _db;

    public CreateBookingCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<BookingResultDto> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        // Serializable transaction to prevent double booking under concurrency
        var txOptions = new TransactionOptions
        {
            IsolationLevel = System.Transactions.IsolationLevel.Serializable,
            Timeout = TimeSpan.FromSeconds(30)
        };

        using var scope = new TransactionScope(
            TransactionScopeOption.Required,
            txOptions,
            TransactionScopeAsyncFlowOption.Enabled);

        // 1) Showtime exists & active
        var showtime = await _db.Showtimes
            .FirstOrDefaultAsync(s => s.Id == request.ShowtimeId, cancellationToken);

        if (showtime is null)
            throw new KeyNotFoundException("Showtime not found.");

        if (!showtime.IsActive)
            throw new InvalidOperationException("Showtime is not active.");

        // 2) Seat exists & belongs to showtime hall
        var seat = await _db.Seats
            .FirstOrDefaultAsync(s => s.Id == request.SeatId, cancellationToken);

        if (seat is null)
            throw new KeyNotFoundException("Seat not found.");

        if (seat.HallId != showtime.HallId)
            throw new InvalidOperationException("Seat does not belong to the showtime hall.");

        // 3) Ensure seat is not already reserved/confirmed
        var existsActiveTicket = await _db.Tickets.AnyAsync(t =>
            t.ShowtimeId == request.ShowtimeId &&
            t.SeatId == request.SeatId &&
            t.Status != TicketStatus.Cancelled,
            cancellationToken);

        if (existsActiveTicket)
            throw new InvalidOperationException("Seat is already reserved.");

        // 4) Create ticket as Pending reservation
        var ticket = new Ticket
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            ShowtimeId = request.ShowtimeId,
            SeatId = request.SeatId,
            HolderName = (request.HolderName ?? string.Empty).Trim(),
            TicketNumber = GenerateTicketNumber(),
            Price = CalculatePrice(showtime.BasePrice, seat.PriceMultiplier),
            Status = TicketStatus.Pending
        };

        _db.Tickets.Add(ticket);
        await _db.SaveChangesAsync(cancellationToken);

        // commit
        scope.Complete();

        return new BookingResultDto(
            TicketId: ticket.Id,
            TicketNumber: ticket.TicketNumber,
            ShowtimeId: ticket.ShowtimeId,
            SeatId: ticket.SeatId,
            Price: ticket.Price,
            Status: ticket.Status
        );
    }

    private static decimal CalculatePrice(decimal basePrice, decimal multiplier)
    {
        var price = basePrice * multiplier;

        // محافظه‌کارانه
        if (price < 0)
            price = 0;

        return decimal.Round(price, 2, MidpointRounding.AwayFromZero);
    }

    private static string GenerateTicketNumber()
    {
        // Example: 20260108-<10 chars>
        var suffix = Guid.NewGuid().ToString("N")[..10].ToUpperInvariant();
        return $"{DateTime.UtcNow:yyyyMMdd}-{suffix}";
    }
}

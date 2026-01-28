using CinemaTicket.Domain.Enums;

namespace CinemaTicket.Application.Features.Bookings.DTOs;

public sealed record BookingResultDto(
    Guid TicketId,
    string TicketNumber,
    Guid ShowtimeId,
    Guid SeatId,
    decimal Price,
    TicketStatus Status
);

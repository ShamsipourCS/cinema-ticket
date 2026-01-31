namespace CinemaTicket.Application.Features.Tickets.DTOs;

using CinemaTicket.Domain.Enums;

public sealed record TicketDto(
    Guid Id,
    string TicketNumber,
    Guid ShowtimeId,
    Guid SeatId,
    decimal Price,
    TicketStatus Status,
    DateTime CreatedAt
);

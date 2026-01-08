namespace CinemaTicket.Application.Features.Showtimes.DTOs;


public sealed record ShowtimeListItemDto(
    Guid Id,
    Guid MovieId,
    Guid HallId,
    DateTime StartTime,
    DateTime EndTime,
    decimal BasePrice,
    bool IsActive
);

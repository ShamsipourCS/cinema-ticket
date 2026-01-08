namespace CinemaTicket.Application.Features.Seats.DTOs;

public sealed record SeatAvailabilityDto(
    Guid SeatId,
    string Label,
    bool IsAvailable
);

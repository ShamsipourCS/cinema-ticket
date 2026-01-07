namespace CinemaTicket.Application.Features.Halls.DTOs;

public sealed record HallDto(
    Guid Id,
    Guid CinemaId,
    string Name,
    int Rows,
    int SeatsPerRow,
    int TotalCapacity
);

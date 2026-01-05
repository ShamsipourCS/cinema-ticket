namespace CinemaTicket.Application.Features.Cinemas.DTOs;

public sealed record CinemaDto(
    Guid Id,
    string Name,
    string Address,
    string City,
    string Phone,
    bool IsActive
);

using MediatR;
using CinemaTicket.Application.Features.Showtimes.DTOs;

namespace CinemaTicket.Application.Features.Showtimes.Queries.GetShowtimes;

public sealed record GetShowtimesQuery(
    Guid? MovieId,
    Guid? HallId,
    DateTime? From,
    DateTime? To,
    bool? IsActive
) : IRequest<IReadOnlyList<ShowtimeDto>>;

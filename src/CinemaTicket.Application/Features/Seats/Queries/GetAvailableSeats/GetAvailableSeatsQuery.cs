using MediatR;
using CinemaTicket.Application.Features.Seats.DTOs;

namespace CinemaTicket.Application.Features.Seats.Queries.GetAvailableSeats;

public sealed record GetAvailableSeatsQuery(Guid ShowtimeId) : IRequest<IReadOnlyList<SeatAvailabilityDto>>;

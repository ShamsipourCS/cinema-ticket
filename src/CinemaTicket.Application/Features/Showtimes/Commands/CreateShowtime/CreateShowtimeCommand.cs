using MediatR;
using CinemaTicket.Application.Features.Showtimes.DTOs;

namespace CinemaTicket.Application.Features.Showtimes.Commands.CreateShowtime;

public sealed record CreateShowtimeCommand(
    Guid MovieId,
    Guid HallId,
    DateTime StartTime,
    DateTime EndTime,
    decimal BasePrice
) : IRequest<ShowtimeDto>;

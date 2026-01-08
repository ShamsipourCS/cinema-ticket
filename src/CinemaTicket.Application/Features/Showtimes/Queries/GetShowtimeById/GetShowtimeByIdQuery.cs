using MediatR;
using CinemaTicket.Application.Features.Showtimes.DTOs;

namespace CinemaTicket.Application.Features.Showtimes.Queries.GetShowtimeById;

public sealed record GetShowtimeByIdQuery(Guid ShowtimeId) : IRequest<ShowtimeDto>;

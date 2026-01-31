using MediatR;

namespace CinemaTicket.Application.Features.Showtimes.Commands.DeleteShowtime;

public sealed record DeleteShowtimeCommand(Guid ShowtimeId) : IRequest;

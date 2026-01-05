using MediatR;

namespace CinemaTicket.Application.Features.Cinemas.Commands.DeleteCinema;

public sealed record DeleteCinemaCommand(Guid Id) : IRequest;

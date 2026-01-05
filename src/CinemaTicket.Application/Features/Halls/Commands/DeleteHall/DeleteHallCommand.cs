using MediatR;

namespace CinemaTicket.Application.Features.Halls.Commands.DeleteHall;

public sealed record DeleteHallCommand(Guid Id) : IRequest;

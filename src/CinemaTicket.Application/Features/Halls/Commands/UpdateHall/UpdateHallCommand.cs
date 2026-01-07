using MediatR;

namespace CinemaTicket.Application.Features.Halls.Commands.UpdateHall;

public sealed record UpdateHallCommand(
    Guid Id,
    string Name,
    int Rows,
    int SeatsPerRow
) : IRequest;

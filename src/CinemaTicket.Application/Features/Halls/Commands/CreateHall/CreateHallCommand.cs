using MediatR;

namespace CinemaTicket.Application.Features.Halls.Commands.CreateHall;

public sealed record CreateHallCommand(
    Guid CinemaId,
    string Name,
    int Rows,
    int SeatsPerRow
) : IRequest<Guid>;

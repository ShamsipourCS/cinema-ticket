using MediatR;

namespace CinemaTicket.Application.Features.Cinemas.Commands.CreateCinema;

public sealed record CreateCinemaCommand(
    string Name,
    string Address,
    string City,
    string Phone,
    bool IsActive
) : IRequest<Guid>;

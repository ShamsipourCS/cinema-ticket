using MediatR;

namespace CinemaTicket.Application.Features.Cinemas.Commands.UpdateCinema;

public sealed record UpdateCinemaCommand(
    Guid Id,
    string Name,
    string Address,
    string City,
    string Phone,
    bool IsActive
) : IRequest;

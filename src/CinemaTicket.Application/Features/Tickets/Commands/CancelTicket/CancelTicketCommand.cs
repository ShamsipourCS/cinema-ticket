using MediatR;

namespace CinemaTicket.Application.Features.Tickets.Commands.CancelTicket;

public sealed record CancelTicketCommand(Guid UserId, Guid TicketId) : IRequest;

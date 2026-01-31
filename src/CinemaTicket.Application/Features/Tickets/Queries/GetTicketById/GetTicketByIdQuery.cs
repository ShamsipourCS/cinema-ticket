using MediatR;
using CinemaTicket.Application.Features.Tickets.DTOs;

namespace CinemaTicket.Application.Features.Tickets.Queries.GetTicketById;

public sealed record GetTicketByIdQuery(Guid UserId, Guid TicketId) : IRequest<TicketDto>;

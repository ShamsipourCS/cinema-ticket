using MediatR;
using CinemaTicket.Application.Features.Tickets.DTOs;

namespace CinemaTicket.Application.Features.Tickets.Queries.GetMyTickets;

public sealed record GetMyTicketsQuery(Guid UserId) : IRequest<IReadOnlyList<TicketDto>>;

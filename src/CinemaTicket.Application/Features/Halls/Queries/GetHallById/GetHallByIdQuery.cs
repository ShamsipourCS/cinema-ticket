using CinemaTicket.Application.Features.Halls.DTOs;
using MediatR;

namespace CinemaTicket.Application.Features.Halls.Queries.GetHallById;

public sealed record GetHallByIdQuery(Guid Id) : IRequest<HallDto>;

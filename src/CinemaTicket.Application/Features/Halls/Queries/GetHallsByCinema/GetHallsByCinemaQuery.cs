using CinemaTicket.Application.Features.Halls.DTOs;
using MediatR;

namespace CinemaTicket.Application.Features.Halls.Queries.GetHallsByCinema;

public sealed record GetHallsByCinemaQuery(
    Guid CinemaId,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<List<HallDto>>;

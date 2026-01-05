using CinemaTicket.Application.Features.Cinemas.DTOs;
using MediatR;

namespace CinemaTicket.Application.Features.Cinemas.Queries.GetCinemas;

public sealed record GetCinemasQuery(
    string? City = null,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<List<CinemaDto>>;

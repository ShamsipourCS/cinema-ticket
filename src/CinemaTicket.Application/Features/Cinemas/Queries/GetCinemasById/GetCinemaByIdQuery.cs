using CinemaTicket.Application.Features.Cinemas.DTOs;
using MediatR;

namespace CinemaTicket.Application.Features.Cinemas.Queries.GetCinemaById;

public sealed record GetCinemaByIdQuery(Guid Id) : IRequest<CinemaDto>;

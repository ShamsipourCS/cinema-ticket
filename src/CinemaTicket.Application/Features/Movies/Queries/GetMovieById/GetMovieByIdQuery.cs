using CinemaTicket.Application.Features.Movies.DTOs;
using MediatR;

namespace CinemaTicket.Application.Features.Movies.Queries.GetMovieById;

public sealed record GetMovieByIdQuery(Guid Id) : IRequest<MovieDto>;

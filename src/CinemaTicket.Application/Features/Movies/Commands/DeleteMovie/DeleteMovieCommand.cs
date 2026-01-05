using MediatR;

namespace CinemaTicket.Application.Features.Movies.Commands.DeleteMovie;

public sealed record DeleteMovieCommand(Guid Id) : IRequest;

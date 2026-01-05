using CinemaTicket.Application.Common.Interfaces;
using CinemaTicket.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CinemaTicket.Application.Features.Movies.Commands.DeleteMovie;

public sealed class DeleteMovieCommandHandler : IRequestHandler<DeleteMovieCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteMovieCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteMovieCommand request, CancellationToken cancellationToken)
    {
        var context = _unitOfWork as DbContext
            ?? throw new InvalidOperationException("UnitOfWork must be a DbContext instance");

        var movie = await context.Set<Movie>()
            .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

        if (movie == null)
            throw new KeyNotFoundException($"Movie with id '{request.Id}' was not found.");

        context.Set<Movie>().Remove(movie);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

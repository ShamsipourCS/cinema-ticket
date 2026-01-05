using CinemaTicket.Application.Common.Interfaces;
using CinemaTicket.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CinemaTicket.Application.Features.Movies.Commands.UpdateMovie;

public sealed class UpdateMovieCommandHandler : IRequestHandler<UpdateMovieCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateMovieCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateMovieCommand request, CancellationToken cancellationToken)
    {
        var context = _unitOfWork as DbContext
            ?? throw new InvalidOperationException("UnitOfWork must be a DbContext instance");

        var movie = await context.Set<Movie>()
            .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

        if (movie == null)
            throw new KeyNotFoundException($"Movie with id '{request.Id}' was not found.");

        movie.Title = request.Title;
        movie.Description = request.Description;
        movie.DurationMinutes = request.DurationMinutes;
        movie.Genre = request.Genre;
        movie.Rating = request.Rating;
        movie.PosterUrl = request.PosterUrl;
        movie.ReleaseDate = request.ReleaseDate;
        movie.IsActive = request.IsActive;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

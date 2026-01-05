using CinemaTicket.Application.Common.Interfaces;
using CinemaTicket.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CinemaTicket.Application.Features.Movies.Commands.CreateMovie;

public sealed class CreateMovieCommandHandler : IRequestHandler<CreateMovieCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateMovieCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateMovieCommand request, CancellationToken cancellationToken)
    {
        var context = _unitOfWork as DbContext
            ?? throw new InvalidOperationException("UnitOfWork must be a DbContext instance");

        var movie = new Movie
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            DurationMinutes = request.DurationMinutes,
            Genre = request.Genre,
            Rating = request.Rating,
            PosterUrl = request.PosterUrl,
            ReleaseDate = request.ReleaseDate,
            IsActive = request.IsActive
        };

        context.Set<Movie>().Add(movie);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return movie.Id;
    }
}

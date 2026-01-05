using CinemaTicket.Application.Common.Interfaces;
using CinemaTicket.Domain.Entities;
using MediatR;

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

        await _unitOfWork.Movies.AddAsync(movie, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return movie.Id;
    }
}

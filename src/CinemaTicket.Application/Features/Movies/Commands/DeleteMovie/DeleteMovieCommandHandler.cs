using CinemaTicket.Application.Common.Interfaces;
using MediatR;

namespace CinemaTicket.Application.Features.Movies.Commands.DeleteMovie;

public sealed class DeleteMovieCommandHandler : IRequestHandler<DeleteMovieCommand>
{
    private readonly IMovieRepository _movieRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteMovieCommandHandler(IMovieRepository movieRepository, IUnitOfWork unitOfWork)
    {
        _movieRepository = movieRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteMovieCommand request, CancellationToken cancellationToken)
    {
        var movie = await _movieRepository.GetByIdAsync(request.Id, cancellationToken);

        if (movie == null)
            throw new KeyNotFoundException($"Movie with id '{request.Id}' was not found.");

        await _movieRepository.DeleteAsync(movie, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

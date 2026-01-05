using CinemaTicket.Application.Common.Interfaces;
using MediatR;

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
        var movie = await _unitOfWork.Movies.GetByIdAsync(request.Id, cancellationToken);

        if (movie == null)
            throw new KeyNotFoundException($"Movie with id '{request.Id}' was not found.");

        await _unitOfWork.Movies.DeleteAsync(movie, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

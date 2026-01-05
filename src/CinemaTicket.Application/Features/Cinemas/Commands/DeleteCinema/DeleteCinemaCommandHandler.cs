using CinemaTicket.Application.Common.Interfaces;
using MediatR;

namespace CinemaTicket.Application.Features.Cinemas.Commands.DeleteCinema;

public sealed class DeleteCinemaCommandHandler : IRequestHandler<DeleteCinemaCommand>
{
    private readonly ICinemaRepository _cinemaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCinemaCommandHandler(ICinemaRepository cinemaRepository, IUnitOfWork unitOfWork)
    {
        _cinemaRepository = cinemaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteCinemaCommand request, CancellationToken cancellationToken)
    {
        var cinema = await _cinemaRepository.GetByIdAsync(request.Id, cancellationToken);

        if (cinema == null)
            throw new KeyNotFoundException($"Cinema with id '{request.Id}' was not found.");

        await _cinemaRepository.DeleteAsync(cinema, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

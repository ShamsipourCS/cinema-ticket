using CinemaTicket.Application.Common.Interfaces;
using MediatR;

namespace CinemaTicket.Application.Features.Cinemas.Commands.DeleteCinema;

public sealed class DeleteCinemaCommandHandler : IRequestHandler<DeleteCinemaCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCinemaCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteCinemaCommand request, CancellationToken cancellationToken)
    {
        var cinema = await _unitOfWork.Cinemas.GetByIdAsync(request.Id, cancellationToken);

        if (cinema == null)
            throw new KeyNotFoundException($"Cinema with id '{request.Id}' was not found.");

        await _unitOfWork.Cinemas.DeleteAsync(cinema, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

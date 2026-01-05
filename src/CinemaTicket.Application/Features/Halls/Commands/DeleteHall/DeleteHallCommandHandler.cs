using CinemaTicket.Application.Common.Interfaces;
using MediatR;

namespace CinemaTicket.Application.Features.Halls.Commands.DeleteHall;

public sealed class DeleteHallCommandHandler : IRequestHandler<DeleteHallCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteHallCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(DeleteHallCommand request, CancellationToken cancellationToken)
    {
        var hall = await _unitOfWork.Halls.GetByIdAsync(request.Id, cancellationToken);

        if (hall == null)
            throw new KeyNotFoundException($"Hall with id '{request.Id}' was not found.");

        await _unitOfWork.Halls.DeleteAsync(hall, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

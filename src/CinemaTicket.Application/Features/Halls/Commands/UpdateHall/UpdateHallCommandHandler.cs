using CinemaTicket.Domain.Interfaces;
using MediatR;

namespace CinemaTicket.Application.Features.Halls.Commands.UpdateHall;

public sealed class UpdateHallCommandHandler : IRequestHandler<UpdateHallCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateHallCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateHallCommand request, CancellationToken cancellationToken)
    {
        var hall = await _unitOfWork.Halls.GetByIdAsync(request.Id, cancellationToken);

        if (hall == null)
            throw new KeyNotFoundException($"Hall with id '{request.Id}' was not found.");

        hall.Name = request.Name;
        hall.Rows = request.Rows;
        hall.SeatsPerRow = request.SeatsPerRow;
        hall.TotalCapacity = request.Rows * request.SeatsPerRow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

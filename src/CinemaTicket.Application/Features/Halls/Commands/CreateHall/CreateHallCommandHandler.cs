using CinemaTicket.Domain.Entities;
using CinemaTicket.Domain.Interfaces;
using MediatR;

namespace CinemaTicket.Application.Features.Halls.Commands.CreateHall;

public sealed class CreateHallCommandHandler : IRequestHandler<CreateHallCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateHallCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateHallCommand request, CancellationToken cancellationToken)
    {
        var hall = new Hall
        {
            Id = Guid.NewGuid(),
            CinemaId = request.CinemaId,
            Name = request.Name,
            Rows = request.Rows,
            SeatsPerRow = request.SeatsPerRow,
            TotalCapacity = request.Rows * request.SeatsPerRow
        };

        await _unitOfWork.Halls.AddAsync(hall, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return hall.Id;
    }
}

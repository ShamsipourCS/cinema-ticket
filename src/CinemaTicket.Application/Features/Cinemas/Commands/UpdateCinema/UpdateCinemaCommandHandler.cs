using CinemaTicket.Domain.Interfaces;
using MediatR;

namespace CinemaTicket.Application.Features.Cinemas.Commands.UpdateCinema;

public sealed class UpdateCinemaCommandHandler : IRequestHandler<UpdateCinemaCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCinemaCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateCinemaCommand request, CancellationToken cancellationToken)
    {
        var cinema = await _unitOfWork.Cinemas.GetByIdAsync(request.Id, cancellationToken);

        if (cinema == null)
            throw new KeyNotFoundException($"Cinema with id '{request.Id}' was not found.");

        cinema.Name = request.Name;
        cinema.Address = request.Address;
        cinema.City = request.City;
        cinema.Phone = request.Phone;
        cinema.IsActive = request.IsActive;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

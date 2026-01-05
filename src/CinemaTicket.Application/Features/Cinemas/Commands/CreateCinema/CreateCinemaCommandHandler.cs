using CinemaTicket.Application.Common.Interfaces;
using CinemaTicket.Domain.Entities;
using MediatR;

namespace CinemaTicket.Application.Features.Cinemas.Commands.CreateCinema;

public sealed class CreateCinemaCommandHandler : IRequestHandler<CreateCinemaCommand, Guid>
{
    private readonly ICinemaRepository _cinemaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCinemaCommandHandler(ICinemaRepository cinemaRepository, IUnitOfWork unitOfWork)
    {
        _cinemaRepository = cinemaRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateCinemaCommand request, CancellationToken cancellationToken)
    {
        var cinema = new Cinema
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Address = request.Address,
            City = request.City,
            Phone = request.Phone,
            IsActive = request.IsActive
        };

        await _cinemaRepository.AddAsync(cinema, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return cinema.Id;
    }
}

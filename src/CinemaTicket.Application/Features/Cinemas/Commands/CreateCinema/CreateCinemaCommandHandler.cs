using CinemaTicket.Domain.Interfaces;
using CinemaTicket.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CinemaTicket.Application.Features.Cinemas.Commands.CreateCinema;

public sealed class CreateCinemaCommandHandler : IRequestHandler<CreateCinemaCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateCinemaCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateCinemaCommand request, CancellationToken cancellationToken)
    {
        var context = _unitOfWork as DbContext
            ?? throw new InvalidOperationException("UnitOfWork must be a DbContext instance");

        var cinema = new Cinema
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Address = request.Address,
            City = request.City,
            Phone = request.Phone,
            IsActive = request.IsActive
        };

        context.Set<Cinema>().Add(cinema);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return cinema.Id;
    }
}

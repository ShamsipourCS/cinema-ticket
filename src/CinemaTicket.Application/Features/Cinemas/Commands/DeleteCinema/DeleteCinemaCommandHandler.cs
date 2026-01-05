using CinemaTicket.Application.Common.Interfaces;
using CinemaTicket.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
        var context = _unitOfWork as DbContext
            ?? throw new InvalidOperationException("UnitOfWork must be a DbContext instance");

        var cinema = await context.Set<Cinema>()
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (cinema == null)
            throw new KeyNotFoundException($"Cinema with id '{request.Id}' was not found.");

        context.Set<Cinema>().Remove(cinema);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

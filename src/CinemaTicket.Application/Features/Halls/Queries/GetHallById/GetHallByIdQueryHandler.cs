using CinemaTicket.Application.Features.Halls.DTOs;
using CinemaTicket.Domain.Interfaces;
using MediatR;

namespace CinemaTicket.Application.Features.Halls.Queries.GetHallById;

public sealed class GetHallByIdQueryHandler : IRequestHandler<GetHallByIdQuery, HallDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetHallByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<HallDto> Handle(GetHallByIdQuery request, CancellationToken cancellationToken)
    {
        var hall = await _unitOfWork.Halls.GetByIdAsync(request.Id, cancellationToken);

        if (hall == null)
            throw new KeyNotFoundException($"Hall with id '{request.Id}' was not found.");

        return new HallDto(
            Id: hall.Id,
            CinemaId: hall.CinemaId,
            Name: hall.Name,
            Rows: hall.Rows,
            SeatsPerRow: hall.SeatsPerRow,
            TotalCapacity: hall.TotalCapacity
        );
    }
}

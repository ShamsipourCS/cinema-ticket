using CinemaTicket.Application.Common.Interfaces;
using CinemaTicket.Application.Features.Halls.DTOs;
using MediatR;

namespace CinemaTicket.Application.Features.Halls.Queries.GetHallsByCinema;

public sealed class GetHallsByCinemaQueryHandler : IRequestHandler<GetHallsByCinemaQuery, List<HallDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetHallsByCinemaQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<HallDto>> Handle(GetHallsByCinemaQuery request, CancellationToken cancellationToken)
    {
        var halls = await _unitOfWork.Halls.FindAsync(h => h.CinemaId == request.CinemaId, cancellationToken);

        var paged = halls
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return paged.Select(h => new HallDto(
            Id: h.Id,
            CinemaId: h.CinemaId,
            Name: h.Name,
            Rows: h.Rows,
            SeatsPerRow: h.SeatsPerRow,
            TotalCapacity: h.TotalCapacity
        )).ToList();
    }
}

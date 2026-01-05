using CinemaTicket.Application.Common.Interfaces;
using CinemaTicket.Application.Features.Cinemas.DTOs;
using CinemaTicket.Domain.Entities;
using MediatR;

namespace CinemaTicket.Application.Features.Cinemas.Queries.GetCinemas;

public sealed class GetCinemasQueryHandler : IRequestHandler<GetCinemasQuery, List<CinemaDto>>
{
        IUnitOfWork _unitOfWork;
    public GetCinemasQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<CinemaDto>> Handle(GetCinemasQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Cinema> cinemas;

        if (!string.IsNullOrWhiteSpace(request.City))
        {
            cinemas = await _unitOfWork.Cinemas.FindAsync(c => c.City == request.City, cancellationToken);
        }
        else
        {
            cinemas = await _unitOfWork.Cinemas.GetAllAsync(cancellationToken);
        }

        var paged = cinemas
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return paged.Select(c => new CinemaDto(
            Id: c.Id,
            Name: c.Name,
            Address: c.Address,
            City: c.City,
            Phone: c.Phone,
            IsActive: c.IsActive
        )).ToList();
    }
}

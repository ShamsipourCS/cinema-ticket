using CinemaTicket.Application.Common.Interfaces;
using CinemaTicket.Application.Features.Cinemas.DTOs;
using CinemaTicket.Domain.Entities;
using MediatR;

namespace CinemaTicket.Application.Features.Cinemas.Queries.GetCinemas;

public sealed class GetCinemasQueryHandler : IRequestHandler<GetCinemasQuery, List<CinemaDto>>
{
    private readonly ICinemaRepository _cinemaRepository;

    public GetCinemasQueryHandler(ICinemaRepository cinemaRepository)
    {
        _cinemaRepository = cinemaRepository;
    }

    public async Task<List<CinemaDto>> Handle(GetCinemasQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Cinema> cinemas;

        if (!string.IsNullOrWhiteSpace(request.City))
        {
            cinemas = await _cinemaRepository.FindAsync(c => c.City == request.City, cancellationToken);
        }
        else
        {
            cinemas = await _cinemaRepository.GetAllAsync(cancellationToken);
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

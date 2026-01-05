using CinemaTicket.Application.Common.Interfaces;
using CinemaTicket.Application.Features.Cinemas.DTOs;
using MediatR;

namespace CinemaTicket.Application.Features.Cinemas.Queries.GetCinemaById;

public sealed class GetCinemaByIdQueryHandler : IRequestHandler<GetCinemaByIdQuery, CinemaDto>
{
    private readonly ICinemaRepository _cinemaRepository;

    public GetCinemaByIdQueryHandler(ICinemaRepository cinemaRepository)
    {
        _cinemaRepository = cinemaRepository;
    }

    public async Task<CinemaDto> Handle(GetCinemaByIdQuery request, CancellationToken cancellationToken)
    {
        var cinema = await _cinemaRepository.GetByIdAsync(request.Id, cancellationToken);

        if (cinema == null)
            throw new KeyNotFoundException($"Cinema with id '{request.Id}' was not found.");

        return new CinemaDto(
            Id: cinema.Id,
            Name: cinema.Name,
            Address: cinema.Address,
            City: cinema.City,
            Phone: cinema.Phone,
            IsActive: cinema.IsActive
        );
    }
}

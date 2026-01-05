using CinemaTicket.Application.Common.Interfaces;
using CinemaTicket.Application.Features.Cinemas.DTOs;
using CinemaTicket.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CinemaTicket.Application.Features.Cinemas.Queries.GetCinemas;

public sealed class GetCinemasByIdQueryHandler : IRequestHandler<GetCinemasQuery, List<CinemaDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCinemasByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<CinemaDto>> Handle(GetCinemasQuery request, CancellationToken cancellationToken)
    {
        var context = _unitOfWork as DbContext
            ?? throw new InvalidOperationException("UnitOfWork must be a DbContext instance");

        var query = context.Set<Cinema>().AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.City))
            query = query.Where(c => c.City == request.City);

        var cinemas = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return cinemas.Select(c => new CinemaDto(
            Id: c.Id,
            Name: c.Name,
            Address: c.Address,
            City: c.City,
            Phone: c.Phone,
            IsActive: c.IsActive
        )).ToList();
    }
}

using FluentValidation;

namespace CinemaTicket.Application.Features.Halls.Queries.GetHallsByCinema;

public sealed class GetHallsByCinemaQueryValidator : AbstractValidator<GetHallsByCinemaQuery>
{
    public GetHallsByCinemaQueryValidator()
    {
        RuleFor(x => x.CinemaId).NotEmpty();
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}

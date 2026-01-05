using FluentValidation;

namespace CinemaTicket.Application.Features.Cinemas.Queries.GetCinemas;

public sealed class GetCinemasByIdQueryValidator : AbstractValidator<GetCinemasQuery>
{
    public GetCinemasByIdQueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);

        When(x => x.City != null, () =>
        {
            RuleFor(x => x.City).MaximumLength(100);
        });
    }
}

using FluentValidation;

namespace CinemaTicket.Application.Features.Cinemas.Queries.GetCinemaById;

public sealed class GetCinemaByIdQueryValidator : AbstractValidator<GetCinemaByIdQuery>
{
    public GetCinemaByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

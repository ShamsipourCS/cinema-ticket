using FluentValidation;

namespace CinemaTicket.Application.Features.Halls.Queries.GetHallById;

public sealed class GetHallByIdQueryValidator : AbstractValidator<GetHallByIdQuery>
{
    public GetHallByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

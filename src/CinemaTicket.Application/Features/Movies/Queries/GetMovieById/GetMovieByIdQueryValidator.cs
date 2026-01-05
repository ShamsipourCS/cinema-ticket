using FluentValidation;

namespace CinemaTicket.Application.Features.Movies.Queries.GetMovieById;

public sealed class GetMovieByIdQueryValidator : AbstractValidator<GetMovieByIdQuery>
{
    public GetMovieByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

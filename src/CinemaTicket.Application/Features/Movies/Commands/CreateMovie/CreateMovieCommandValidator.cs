using FluentValidation;

namespace CinemaTicket.Application.Features.Movies.Commands.CreateMovie;

public sealed class CreateMovieCommandValidator : AbstractValidator<CreateMovieCommand>
{
    public CreateMovieCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.DurationMinutes).GreaterThan(0).LessThanOrEqualTo(600);
        RuleFor(x => x.Genre).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Rating).NotEmpty().MaximumLength(20);
        RuleFor(x => x.PosterUrl).NotEmpty().MaximumLength(500);
        RuleFor(x => x.ReleaseDate).NotEmpty();
    }
}

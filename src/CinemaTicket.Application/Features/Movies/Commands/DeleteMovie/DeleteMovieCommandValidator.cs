using FluentValidation;

namespace CinemaTicket.Application.Features.Movies.Commands.DeleteMovie;

public sealed class DeleteMovieCommandValidator : AbstractValidator<DeleteMovieCommand>
{
    public DeleteMovieCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

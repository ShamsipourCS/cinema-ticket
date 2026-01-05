using FluentValidation;

namespace CinemaTicket.Application.Features.Cinemas.Commands.DeleteCinema;

public sealed class DeleteCinemaCommandValidator : AbstractValidator<DeleteCinemaCommand>
{
    public DeleteCinemaCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

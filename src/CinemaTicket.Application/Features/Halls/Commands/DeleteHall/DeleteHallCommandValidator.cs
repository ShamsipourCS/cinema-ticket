using FluentValidation;

namespace CinemaTicket.Application.Features.Halls.Commands.DeleteHall;

public sealed class DeleteHallCommandValidator : AbstractValidator<DeleteHallCommand>
{
    public DeleteHallCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

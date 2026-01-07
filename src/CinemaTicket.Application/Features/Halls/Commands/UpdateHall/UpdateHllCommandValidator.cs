using FluentValidation;

namespace CinemaTicket.Application.Features.Halls.Commands.UpdateHall;

public sealed class UpdateHallCommandValidator : AbstractValidator<UpdateHallCommand>
{
    public UpdateHallCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Rows).GreaterThan(0).LessThanOrEqualTo(100);
        RuleFor(x => x.SeatsPerRow).GreaterThan(0).LessThanOrEqualTo(100);
    }
}
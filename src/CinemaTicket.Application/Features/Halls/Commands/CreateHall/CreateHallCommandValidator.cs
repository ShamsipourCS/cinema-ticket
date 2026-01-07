using FluentValidation;

namespace CinemaTicket.Application.Features.Halls.Commands.CreateHall;

public sealed class CreateHallCommandValidator : AbstractValidator<CreateHallCommand>
{
    public CreateHallCommandValidator()
    {
        RuleFor(x => x.CinemaId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Rows).GreaterThan(0).LessThanOrEqualTo(100);
        RuleFor(x => x.SeatsPerRow).GreaterThan(0).LessThanOrEqualTo(100);
    }
}

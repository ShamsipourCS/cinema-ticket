using FluentValidation;

namespace CinemaTicket.Application.Features.Showtimes.Commands.CreateShowtime;

public sealed class CreateShowtimeCommandValidator : AbstractValidator<CreateShowtimeCommand>
{
    public CreateShowtimeCommandValidator()
    {
        RuleFor(x => x.MovieId).NotEmpty();
        RuleFor(x => x.HallId).NotEmpty();
        RuleFor(x => x.StartTime).NotEmpty();
        RuleFor(x => x.EndTime).NotEmpty().GreaterThan(x => x.StartTime);
        RuleFor(x => x.BasePrice).GreaterThanOrEqualTo(0);
    }
}

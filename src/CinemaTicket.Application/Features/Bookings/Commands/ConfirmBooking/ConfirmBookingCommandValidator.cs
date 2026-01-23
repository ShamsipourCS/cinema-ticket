using FluentValidation;

namespace CinemaTicket.Application.Features.Bookings.Commands.ConfirmBooking;

public sealed class ConfirmBookingCommandValidator : AbstractValidator<ConfirmBookingCommand>
{
    public ConfirmBookingCommandValidator()
    {
        RuleFor(x => x.StripePaymentIntentId)
            .NotEmpty()
            .WithMessage("Stripe payment intent ID is required")
            .Must(id => id.StartsWith("pi_"))
            .WithMessage("Invalid Stripe payment intent ID format")
            .MaximumLength(200)
            .WithMessage("Stripe payment intent ID cannot exceed 200 characters");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.ShowtimeId)
            .NotEmpty()
            .WithMessage("Showtime ID is required");

        RuleFor(x => x.SeatId)
            .NotEmpty()
            .WithMessage("Seat ID is required");

        RuleFor(x => x.HolderName)
            .NotEmpty()
            .WithMessage("Holder name is required")
            .MinimumLength(2)
            .WithMessage("Holder name must be at least 2 characters")
            .MaximumLength(100)
            .WithMessage("Holder name cannot exceed 100 characters");
    }
}

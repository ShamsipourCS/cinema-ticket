using FluentValidation;

namespace CinemaTicket.Application.Features.Payments.Commands.CreatePaymentIntent;

/// <summary>
/// Validator for CreatePaymentIntentCommand.
/// </summary>
public sealed class CreatePaymentIntentCommandValidator : AbstractValidator<CreatePaymentIntentCommand>
{
    public CreatePaymentIntentCommandValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThanOrEqualTo(50).WithMessage("Amount must be at least 50 cents (Stripe minimum).");

        RuleFor(x => x.Currency)
            .Length(3).WithMessage("Currency must be a 3-letter ISO code (e.g., 'usd')")
            .Must(c => c == null || c == c.ToLower()).WithMessage("Currency must be lowercase")
            .When(x => !string.IsNullOrEmpty(x.Currency));
    }
}

using FluentValidation;

namespace CinemaTicket.Application.Features.Auth.Commands.RefreshToken;

/// <summary>
/// Validator for RefreshTokenCommand.
/// </summary>
public sealed class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.AccessToken)
            .NotEmpty();

        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .MinimumLength(32).WithMessage("Refresh token must be at least 32 characters");
    }
}

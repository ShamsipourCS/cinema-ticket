using FluentValidation;

namespace CinemaTicket.Application.Features.Movies.Queries.GetMovies;

/// <summary>
/// Validator for the GetMoviesQuery to ensure valid pagination and filter parameters.
/// Demonstrates the ValidationBehavior intercepting invalid requests before they reach the handler.
/// </summary>
public class GetMoviesQueryValidator : AbstractValidator<GetMoviesQuery>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetMoviesQueryValidator"/> class
    /// and configures validation rules.
    /// </summary>
    public GetMoviesQueryValidator()
    {
        // PageNumber must be at least 1
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page number must be at least 1.");

        // PageSize must be between 1 and 100
        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Page size must be between 1 and 100.");

        // Genre, if provided, must not be empty or whitespace
        When(x => !string.IsNullOrWhiteSpace(x.Genre), () =>
        {
            RuleFor(x => x.Genre)
                .NotEmpty()
                .WithMessage("Genre cannot be empty if provided.");
        });
    }
}

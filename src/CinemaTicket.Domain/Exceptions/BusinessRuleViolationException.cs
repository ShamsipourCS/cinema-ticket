namespace CinemaTicket.Domain.Exceptions;

/// <summary>
/// Exception thrown when a business rule is violated
/// </summary>
public class BusinessRuleViolationException : DomainException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BusinessRuleViolationException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the business rule violation.</param>
    public BusinessRuleViolationException(string message) : base(message) { }
}

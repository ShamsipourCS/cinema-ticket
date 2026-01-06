namespace CinemaTicket.Domain.Exceptions;

/// <summary>
/// Exception thrown when a conflict occurs (e.g., duplicate resource)
/// </summary>
public class ConflictException : DomainException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConflictException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the conflict.</param>
    public ConflictException(string message) : base(message) { }
}

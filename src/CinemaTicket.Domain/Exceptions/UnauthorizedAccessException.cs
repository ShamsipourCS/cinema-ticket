namespace CinemaTicket.Domain.Exceptions;

/// <summary>
/// Exception thrown when unauthorized access is attempted
/// </summary>
public class UnauthorizedAccessException : DomainException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnauthorizedAccessException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the unauthorized access attempt.</param>
    public UnauthorizedAccessException(string message)
        : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="UnauthorizedAccessException"/> class with a default error message.
    /// </summary>
    public UnauthorizedAccessException()
        : base("Unauthorized access attempted.") { }
}

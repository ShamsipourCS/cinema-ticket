namespace CinemaTicket.Domain.Exceptions;

/// <summary>
/// Exception thrown when a requested entity is not found
/// </summary>
public class NotFoundException : DomainException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundException"/> class with entity name and identifier.
    /// </summary>
    /// <param name="entityName">The name of the entity that was not found.</param>
    /// <param name="key">The identifier used to search for the entity.</param>
    public NotFoundException(string entityName, object key)
        : base($"{entityName} with identifier '{key}' was not found.") { }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundException"/> class with a custom error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public NotFoundException(string message) : base(message) { }
}

using System.ComponentModel.DataAnnotations;
using CinemaTicket.Domain.Common;
using CinemaTicket.Domain.Enums;

namespace CinemaTicket.Domain.Entities;

/// <summary>
/// Represents a user in the system.
/// </summary>
public class User : AuditableEntity
{
    /// <summary>
    /// Gets or sets the user's email address.
    /// Must be a valid email format.
    /// </summary>
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the hashed password of the user.
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's first name.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's last name.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the user's phone number.
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the role assigned to the user.
    /// </summary>
    public UserRole Role { get; set; } = UserRole.User;
    
    /// <summary>
    /// Gets or sets the collection of refresh tokens associated with the user.
    /// </summary>
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    /// <summary>
    /// Gets or sets the collection of tickets purchased by the user.
    /// </summary>
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}

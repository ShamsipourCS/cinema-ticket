namespace CinemaTicket.Domain.Enums;

/// <summary>
/// Defines the roles available for users in the system.
/// </summary>
public enum UserRole
{
    /// <summary>
    /// A regular customer who can book tickets.
    /// </summary>
    User = 0,

    /// <summary>
    /// An administrator who can manage cinemas, movies, and showtimes.
    /// </summary>
    Admin = 1
}

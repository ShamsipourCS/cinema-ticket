using CinemaTicket.Domain.Common;
using CinemaTicket.Domain.Enums;

namespace CinemaTicket.Domain.Entities;

public class User : AuditableEntity
{
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.User;
    
    // Navigation Properties
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    // public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}

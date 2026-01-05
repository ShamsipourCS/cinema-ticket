using CinemaTicket.Domain.Common;

namespace CinemaTicket.Domain.Entities;

/// <summary>
/// Represents a cinema location.
/// </summary>
public class Cinema : BaseEntity
{
    /// <summary>
    /// Gets or sets the name of the cinema.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the physical address of the cinema.
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the city where the cinema is located.
    /// </summary>
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the contact phone number for the cinema.
    /// </summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the cinema is currently operational.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets the collection of halls belonging to this cinema.
    /// </summary>
    public ICollection<Hall> Halls { get; set; } = new List<Hall>();
}

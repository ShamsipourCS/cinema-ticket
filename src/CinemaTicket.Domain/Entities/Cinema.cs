using CinemaTicket.Domain.Common;

namespace CinemaTicket.Domain.Entities;

public class Cinema : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public bool IsActive { get; set; }

    // Navigation Properties
    public ICollection<Hall> Halls { get; set; } = new List<Hall>();
}

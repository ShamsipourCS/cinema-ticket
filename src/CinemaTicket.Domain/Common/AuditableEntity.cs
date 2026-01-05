using System;

namespace CinemaTicket.Domain.Common;

/// <summary>
/// Base class for entities that require auditing information.
/// </summary>
public abstract class AuditableEntity : BaseEntity
{
    /// <summary>
    /// Gets or sets the date and time when the entity was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

using System;

namespace CinemaTicket.Domain.Common;

/// <summary>
/// Base class for all entities in the system, providing a unique identifier.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
}

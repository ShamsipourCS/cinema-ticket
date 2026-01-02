namespace CinemaTicket.Domain.Enums;

/// <summary>
/// Defines the types of seats available in a cinema hall.
/// </summary>
public enum SeatType
{
    /// <summary>
    /// A standard seat.
    /// </summary>
    Regular = 0,

    /// <summary>
    /// A premium seat with extra comfort or better view.
    /// </summary>
    Vip = 1,

    /// <summary>
    /// A seat designated for individuals with accessibility needs.
    /// </summary>
    Accessible = 2
}

using Microsoft.AspNetCore.Authorization;

namespace CinemaTicket.API.Authorization;

/// <summary>
/// Authorization requirement for validating ticket ownership.
/// Used to ensure users can only access their own tickets.
/// </summary>
public class TicketOwnershipRequirement : IAuthorizationRequirement
{
}

using System.Security.Claims;
using CinemaTicket.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace CinemaTicket.API.Authorization;

/// <summary>
/// Authorization handler for validating ticket ownership.
/// Ensures users can only access their own tickets, with admin bypass.
/// </summary>
public class TicketOwnershipHandler : AuthorizationHandler<TicketOwnershipRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TicketOwnershipHandler> _logger;

    public TicketOwnershipHandler(
        IHttpContextAccessor httpContextAccessor,
        IUnitOfWork unitOfWork,
        ILogger<TicketOwnershipHandler> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        TicketOwnershipRequirement requirement)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            _logger.LogWarning("HttpContext is null in TicketOwnershipHandler");
            return;
        }

        // Extract ticketId from route values
        var ticketIdString = httpContext.Request.RouteValues["ticketId"]?.ToString();
        if (!Guid.TryParse(ticketIdString, out var ticketId))
        {
            _logger.LogWarning("Invalid or missing ticketId in route");
            return;
        }

        // Extract userId from JWT claims
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            _logger.LogWarning("Invalid or missing user ID in claims");
            return;
        }

        // Admin can access all tickets
        if (context.User.IsInRole("Admin"))
        {
            _logger.LogDebug("Admin user {UserId} accessing ticket {TicketId}", userId, ticketId);
            context.Succeed(requirement);
            return;
        }

        // Verify ownership
        var ticket = await _unitOfWork.Tickets.GetByIdAsync(ticketId);
        if (ticket != null && ticket.UserId == userId)
        {
            _logger.LogDebug("User {UserId} authorized to access their ticket {TicketId}", userId, ticketId);
            context.Succeed(requirement);
        }
        else
        {
            _logger.LogWarning("User {UserId} attempted unauthorized access to ticket {TicketId}",
                userId, ticketId);
            // Authorization fails - returns 403 Forbidden
        }
    }
}

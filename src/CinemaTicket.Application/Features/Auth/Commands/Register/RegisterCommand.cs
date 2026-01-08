using CinemaTicket.Application.Features.Auth.DTOs;
using MediatR;

namespace CinemaTicket.Application.Features.Auth.Commands.Register;

/// <summary>
/// Command for registering a new user.
/// </summary>
/// <param name="Email">User's email address.</param>
/// <param name="Password">User's password (will be hashed before storage).</param>
/// <param name="FirstName">User's first name.</param>
/// <param name="LastName">User's last name.</param>
/// <param name="Phone">User's phone number (optional).</param>
public sealed record RegisterCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string? Phone
) : IRequest<AuthResponseDto>;

using System;

namespace CinemaTicket.Application.Features.Auth.DTOs;

/// <summary>
/// Data transfer object for user information in authentication responses.
/// </summary>
/// <param name="Id">The unique identifier of the user.</param>
/// <param name="Email">The user's email address.</param>
/// <param name="FirstName">The user's first name.</param>
/// <param name="LastName">The user's last name.</param>
/// <param name="Phone">The user's phone number (optional).</param>
/// <param name="Role">The user's role (Customer or Admin).</param>
public sealed record UserDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string? Phone,
    string Role
);

using System;

namespace CinemaTicket.Application.Features.Auth.DTOs;

/// <summary>
/// Data transfer object for authentication responses containing tokens and user information.
/// </summary>
/// <param name="AccessToken">JWT access token for API authentication.</param>
/// <param name="RefreshToken">Refresh token for obtaining new access tokens.</param>
/// <param name="ExpiresAt">UTC timestamp when the access token expires.</param>
/// <param name="User">User information associated with the authentication.</param>
public sealed record AuthResponseDto(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    UserDto User
);

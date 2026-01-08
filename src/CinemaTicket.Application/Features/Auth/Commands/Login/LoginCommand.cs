using CinemaTicket.Application.Features.Auth.DTOs;
using MediatR;

namespace CinemaTicket.Application.Features.Auth.Commands.Login;

/// <summary>
/// Command for user login authentication.
/// </summary>
/// <param name="Email">User's email address.</param>
/// <param name="Password">User's password.</param>
public sealed record LoginCommand(
    string Email,
    string Password
) : IRequest<AuthResponseDto>;

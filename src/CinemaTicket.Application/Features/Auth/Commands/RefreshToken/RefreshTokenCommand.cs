using CinemaTicket.Application.Features.Auth.DTOs;
using MediatR;

namespace CinemaTicket.Application.Features.Auth.Commands.RefreshToken;

/// <summary>
/// Command for refreshing an expired access token using a refresh token.
/// </summary>
/// <param name="AccessToken">The expired access token.</param>
/// <param name="RefreshToken">The refresh token to use for obtaining a new access token.</param>
public sealed record RefreshTokenCommand(
    string AccessToken,
    string RefreshToken
) : IRequest<AuthResponseDto>;

using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using CinemaTicket.Application.Common.Interfaces;
using CinemaTicket.Application.Features.Auth.DTOs;
using CinemaTicket.Domain.Entities;
using CinemaTicket.Domain.Exceptions;
using CinemaTicket.Domain.Interfaces;
using MediatR;

namespace CinemaTicket.Application.Features.Auth.Commands.RefreshToken;

/// <summary>
/// Handler for refreshing an expired access token using a refresh token.
/// Implements token rotation for security: old refresh token is revoked, new one is issued.
/// </summary>
public sealed class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;

    public RefreshTokenCommandHandler(
        IUnitOfWork unitOfWork,
        IJwtService jwtService)
    {
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
    }

    public async Task<AuthResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // Extract claims from expired access token
        var principal = _jwtService.GetPrincipalFromExpiredToken(request.AccessToken);
        if (principal == null)
        {
            throw new Domain.Exceptions.UnauthorizedAccessException("Invalid access token.");
        }

        // Get user ID from claims
        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new Domain.Exceptions.UnauthorizedAccessException("Invalid token claims.");
        }

        // Validate refresh token
        var storedRefreshToken = await _unitOfWork.RefreshTokens.GetValidTokenAsync(
            request.RefreshToken,
            userId,
            cancellationToken);

        if (storedRefreshToken == null)
        {
            throw new Domain.Exceptions.UnauthorizedAccessException("Invalid or expired refresh token.");
        }

        // Get user
        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            throw new Domain.Exceptions.UnauthorizedAccessException("User not found.");
        }

        // Generate new JWT access token
        var newAccessToken = _jwtService.GenerateToken(user);

        // Generate new refresh token
        var newRefreshTokenString = _jwtService.GenerateRefreshToken();
        var newRefreshToken = new Domain.Entities.RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = newRefreshTokenString,
            ExpiresAt = DateTime.UtcNow.AddDays(7), // 7-day refresh token expiration
            IsRevoked = false
        };

        // Revoke old refresh token (token rotation for security)
        storedRefreshToken.IsRevoked = true;

        // Store new refresh token
        await _unitOfWork.RefreshTokens.AddAsync(newRefreshToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Return authentication response
        return new AuthResponseDto(
            AccessToken: newAccessToken,
            RefreshToken: newRefreshTokenString,
            ExpiresAt: DateTime.UtcNow.AddMinutes(60), // Match JWT expiration
            User: new UserDto(
                Id: user.Id,
                Email: user.Email,
                FirstName: user.FirstName,
                LastName: user.LastName,
                Phone: user.Phone,
                Role: user.Role.ToString()
            )
        );
    }
}

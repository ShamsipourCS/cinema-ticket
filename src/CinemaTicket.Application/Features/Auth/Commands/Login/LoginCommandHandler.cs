using System;
using System.Threading;
using System.Threading.Tasks;
using CinemaTicket.Application.Common.Interfaces;
using CinemaTicket.Application.Features.Auth.DTOs;
using CinemaTicket.Domain.Entities;
using CinemaTicket.Domain.Exceptions;
using CinemaTicket.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CinemaTicket.Application.Features.Auth.Commands.Login;

/// <summary>
/// Handler for user login authentication.
/// </summary>
public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly IPasswordHasher<User> _passwordHasher;

    public LoginCommandHandler(
        IUnitOfWork unitOfWork,
        IJwtService _jwtService,
        IPasswordHasher<User> passwordHasher)
    {
        _unitOfWork = unitOfWork;
        this._jwtService = _jwtService;
        _passwordHasher = passwordHasher;
    }

    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // Get user by email
        var user = await _unitOfWork.Users.GetByEmailAsync(request.Email, cancellationToken);
        if (user == null)
        {
            throw new Domain.Exceptions.UnauthorizedAccessException("Invalid credentials.");
        }

        // Verify password
        var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verificationResult == PasswordVerificationResult.Failed)
        {
            throw new Domain.Exceptions.UnauthorizedAccessException("Invalid credentials.");
        }

        // Generate JWT access token
        var accessToken = _jwtService.GenerateToken(user);

        // Generate refresh token
        var refreshTokenString = _jwtService.GenerateRefreshToken();
        var refreshToken = new Domain.Entities.RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = refreshTokenString,
            ExpiresAt = DateTime.UtcNow.AddDays(7), // 7-day refresh token expiration
            IsRevoked = false
        };

        // Store refresh token
        await _unitOfWork.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Return authentication response
        return new AuthResponseDto(
            AccessToken: accessToken,
            RefreshToken: refreshTokenString,
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

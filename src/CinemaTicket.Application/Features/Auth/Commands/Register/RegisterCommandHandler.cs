using System;
using System.Threading;
using System.Threading.Tasks;
using CinemaTicket.Application.Common.Interfaces;
using CinemaTicket.Application.Features.Auth.DTOs;
using CinemaTicket.Domain.Entities;
using CinemaTicket.Domain.Enums;
using CinemaTicket.Domain.Exceptions;
using CinemaTicket.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CinemaTicket.Application.Features.Auth.Commands.Register;

/// <summary>
/// Handler for registering a new user.
/// </summary>
public sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly IPasswordHasher<User> _passwordHasher;

    public RegisterCommandHandler(
        IUnitOfWork unitOfWork,
        IJwtService jwtService,
        IPasswordHasher<User> passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
        _passwordHasher = passwordHasher;
    }

    public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // Check if user already exists
        var existingUser = await _unitOfWork.Users.GetByEmailAsync(request.Email, cancellationToken);
        if (existingUser != null)
        {
            throw new ConflictException($"User with email {request.Email} already exists.");
        }

        // Create new user
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Phone = request.Phone,
            Role = UserRole.Customer // Default role for registration
        };

        // Hash password
        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        // Add user to database
        await _unitOfWork.Users.AddAsync(user, cancellationToken);

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

        // Save all changes
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

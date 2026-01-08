using CinemaTicket.Application.Features.Auth.Commands.Login;
using CinemaTicket.Application.Features.Auth.Commands.RefreshToken;
using CinemaTicket.Application.Features.Auth.Commands.Register;
using CinemaTicket.Application.Features.Auth.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CinemaTicket.API.Controllers;

/// <summary>
/// Controller for authentication operations (register, login, refresh token).
/// All endpoints are public (no authorization required).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AuthController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthController"/> class.
    /// </summary>
    /// <param name="mediator">The MediatR instance for sending commands.</param>
    /// <param name="logger">The logger instance.</param>
    public AuthController(IMediator mediator, ILogger<AuthController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Registers a new user account.
    /// </summary>
    /// <param name="command">The registration command containing user details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Authentication response with access token and user information.</returns>
    /// <response code="200">User registered successfully.</response>
    /// <response code="400">Validation failed (e.g., weak password, invalid email).</response>
    /// <response code="409">User with the email already exists.</response>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/Auth/register
    ///     {
    ///         "email": "john.doe@example.com",
    ///         "password": "SecurePass123!",
    ///         "firstName": "John",
    ///         "lastName": "Doe",
    ///         "phone": "+1234567890"
    ///     }
    ///
    /// Password must:
    /// - Be at least 8 characters
    /// - Contain uppercase letter
    /// - Contain lowercase letter
    /// - Contain digit
    /// - Contain special character
    /// </remarks>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Authenticates a user and returns access tokens.
    /// </summary>
    /// <param name="command">The login command containing credentials.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Authentication response with access token and user information.</returns>
    /// <response code="200">Login successful.</response>
    /// <response code="400">Validation failed (e.g., empty email or password).</response>
    /// <response code="401">Invalid credentials.</response>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/Auth/login
    ///     {
    ///         "email": "john.doe@example.com",
    ///         "password": "SecurePass123!"
    ///     }
    /// </remarks>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromBody] LoginCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Refreshes an expired access token using a refresh token.
    /// Implements token rotation: old refresh token is revoked, new one is issued.
    /// </summary>
    /// <param name="command">The refresh token command containing tokens.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Authentication response with new tokens.</returns>
    /// <response code="200">Token refreshed successfully.</response>
    /// <response code="400">Validation failed.</response>
    /// <response code="401">Invalid or expired refresh token.</response>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/Auth/refresh
    ///     {
    ///         "accessToken": "eyJ...",
    ///         "refreshToken": "base64-encoded-token"
    ///     }
    ///
    /// Security: Old refresh token will be revoked after successful refresh.
    /// </remarks>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken(
        [FromBody] RefreshTokenCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}

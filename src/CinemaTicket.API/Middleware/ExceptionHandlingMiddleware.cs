using CinemaTicket.API.Models;
using CinemaTicket.Domain.Exceptions;
using FluentValidation;
using System.Text.Json;

namespace CinemaTicket.API.Middleware;

/// <summary>
/// Middleware for handling unhandled exceptions globally across the application.
/// Converts domain exceptions to appropriate HTTP responses with standardized error format.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionHandlingMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <param name="logger">The logger service.</param>
    /// <param name="environment">The web host environment service.</param>
    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    /// <summary>
    /// Invokes the middleware to process the HTTP context.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// Handles exceptions and converts them to appropriate HTTP responses.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <param name="exception">The exception that was thrown.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message, errors) = exception switch
        {
            // FluentValidation exceptions (400 Bad Request)
            ValidationException validationException => (
                StatusCodes.Status400BadRequest,
                "One or more validation errors occurred.",
                validationException.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    )
            ),

            // Domain exceptions (404 Not Found)
            NotFoundException notFoundException => (
                StatusCodes.Status404NotFound,
                notFoundException.Message,
                null
            ),

            // Business rule violations (400 Bad Request)
            BusinessRuleViolationException businessException => (
                StatusCodes.Status400BadRequest,
                businessException.Message,
                null
            ),

            // Unauthorized access (401 Unauthorized)
            // Note: Using fully qualified name to avoid conflict with System.UnauthorizedAccessException
            Domain.Exceptions.UnauthorizedAccessException unauthorizedException => (
                StatusCodes.Status401Unauthorized,
                unauthorizedException.Message,
                null
            ),

            // Conflicts (409 Conflict)
            ConflictException conflictException => (
                StatusCodes.Status409Conflict,
                conflictException.Message,
                null
            ),

            // All other exceptions (500 Internal Server Error)
            _ => (
                StatusCodes.Status500InternalServerError,
                "An internal server error occurred.",
                null
            )
        };

        // Log with appropriate severity
        if (statusCode >= 500)
        {
            _logger.LogError(exception,
                "Server error occurred. TraceId: {TraceId}",
                context.TraceIdentifier);
        }
        else if (statusCode >= 400)
        {
            _logger.LogWarning(exception,
                "Client error occurred. TraceId: {TraceId}",
                context.TraceIdentifier);
        }

        var response = new ErrorResponse
        {
            StatusCode = statusCode,
            Message = message,
            Details = _environment.IsDevelopment() ? exception.ToString() : null,
            Errors = errors,
            TraceId = context.TraceIdentifier
        };

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = _environment.IsDevelopment()
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response, options),
            cancellationToken: default);
    }
}

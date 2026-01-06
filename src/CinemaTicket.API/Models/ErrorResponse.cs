namespace CinemaTicket.API.Models;

/// <summary>
/// Standardized error response for all API errors
/// </summary>
public record ErrorResponse
{
    /// <summary>
    /// HTTP status code
    /// </summary>
    public int StatusCode { get; init; }

    /// <summary>
    /// User-friendly error message
    /// </summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>
    /// Detailed error information (only in Development environment)
    /// </summary>
    public string? Details { get; init; }

    /// <summary>
    /// Validation errors dictionary (for 400 Bad Request)
    /// </summary>
    public IDictionary<string, string[]>? Errors { get; init; }

    /// <summary>
    /// Unique trace identifier for log correlation
    /// </summary>
    public string? TraceId { get; init; }

    /// <summary>
    /// Timestamp of the error
    /// </summary>
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}

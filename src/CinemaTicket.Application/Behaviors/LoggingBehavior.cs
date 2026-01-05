using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CinemaTicket.Application.Behaviors;

/// <summary>
/// Pipeline behavior that logs request execution and timing information.
/// Provides basic performance monitoring by measuring handler execution time.
/// </summary>
/// <typeparam name="TRequest">The type of request being logged.</typeparam>
/// <typeparam name="TResponse">The type of response expected from the handler.</typeparam>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoggingBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Handles request logging before and after handler execution.
    /// </summary>
    /// <param name="request">The request being handled.</param>
    /// <param name="next">The next behavior or handler in the pipeline.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The response from the handler.</returns>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var responseName = typeof(TResponse).Name;

        // Log request start
        _logger.LogInformation(
            "Handling {RequestName}",
            requestName
        );

        // Start timing
        var stopwatch = Stopwatch.StartNew();

        try
        {
            // Execute the handler
            var response = await next();

            // Stop timing
            stopwatch.Stop();

            // Log successful completion with timing
            _logger.LogInformation(
                "Handled {RequestName} → {ResponseName} in {ElapsedMilliseconds}ms",
                requestName,
                responseName,
                stopwatch.ElapsedMilliseconds
            );

            return response;
        }
        catch (Exception ex)
        {
            // Stop timing
            stopwatch.Stop();

            // Log error with timing
            _logger.LogError(
                ex,
                "Error handling {RequestName} after {ElapsedMilliseconds}ms: {ErrorMessage}",
                requestName,
                stopwatch.ElapsedMilliseconds,
                ex.Message
            );

            // Re-throw to allow exception handling middleware to handle it
            throw;
        }
    }
}

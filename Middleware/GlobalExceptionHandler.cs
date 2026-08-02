using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace TraineeManagementApi.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // Log the full exception detail for server-side debugging
        _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);

        var (statusCode, title) = exception switch
        {
            KeyNotFoundException => (StatusCodes.Status404NotFound, "Resource Not Found"),
            ArgumentException => (StatusCodes.Status400BadRequest, "Invalid Input Data"),
            InvalidOperationException => (StatusCodes.Status400BadRequest, "Invalid Operation"),
            UnauthorizedAccessException => (StatusCodes.Status403Forbidden, "Forbidden"),
            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error")
        };

        // Only the exceptions above are ones we deliberately throw with safe, user-facing
        // messages (e.g. "Trainee with ID '...' was not found."). Anything else falls through
        // to a generic 500 and must NOT expose exception.Message to the client, since that can
        // include internal details from EF/DbUpdateException, Redis, connection strings, etc.
        // The full exception is still logged above for server-side debugging.
        string clientMessage = statusCode == StatusCodes.Status500InternalServerError
            ? "An unexpected error occurred. Please try again later."
            : exception.Message;

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(new {
            Status = statusCode,
            Title = title,
            Message = clientMessage,
            Instance = httpContext.Request.Path,
        }, cancellationToken);

        return true;
    }
}

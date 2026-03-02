using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Shared.Exceptions;

/// <summary>
/// Global exception handler — catches ALL unhandled exceptions in one place.
///
/// WHY THIS APPROACH (not try/catch in every endpoint):
///
/// 1. DRY — One handler for all endpoints. No copy-paste try/catch everywhere.
/// 2. Consistent response format — Every error returns the same JSON structure (RFC 7807 Problem Details).
/// 3. Separation of concerns — Endpoints focus on business logic, not error formatting.
/// 4. Logging in one place — Every unhandled exception gets logged with full context.
/// 5. Security — In production, you never leak stack traces or internal details to clients.
///
/// WHEN TO USE TRY/CATCH IN ENDPOINTS:
/// - Only for EXPECTED errors that need specific business logic handling
///   Example: "if customer not found, return BadRequest" — that's business logic, not error handling
/// - Never for catching generic exceptions — let them bubble up to this handler
/// </summary>
public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        // Log the full exception with structured data for debugging
        logger.LogError(exception,
            "Unhandled exception on {Method} {Path}",
            httpContext.Request.Method,
            httpContext.Request.Path);

        // Map exception types to HTTP status codes
        var (statusCode, title) = exception switch
        {
            ArgumentException => (StatusCodes.Status400BadRequest, "Bad Request"),
            KeyNotFoundException => (StatusCodes.Status404NotFound, "Not Found"),
            InvalidOperationException => (StatusCodes.Status409Conflict, "Conflict"),
            UnauthorizedAccessException => (StatusCodes.Status403Forbidden, "Forbidden"),
            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error")
        };

        httpContext.Response.StatusCode = statusCode;

        // RFC 7807 Problem Details — standard error format
        await httpContext.Response.WriteAsJsonAsync(new
        {
            status = statusCode,
            title,
            // In production: NEVER expose exception details to clients
            // In development: show the message for debugging
            detail = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"
                ? exception.Message
                : "An error occurred. Check server logs for details."
        }, cancellationToken);

        return true; // We handled it — don't let it propagate further
    }
}

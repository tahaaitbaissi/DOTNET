using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace CarRental.WebApi.Middleware
{
    /// <summary>
    /// Global exception handler that catches all unhandled exceptions
    /// and returns a standardized JSON error response
    /// </summary>
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly IHostEnvironment _environment;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            // Generate a correlation ID for tracking
            var correlationId = Guid.NewGuid().ToString();

            // Log the exception with correlation ID
            _logger.LogError(exception, 
                "Unhandled exception occurred. CorrelationId: {CorrelationId}, Path: {Path}", 
                correlationId, 
                httpContext.Request.Path);

            // Determine status code and message based on exception type
            var (statusCode, title, detail) = MapException(exception);

            // Create problem details response
            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = _environment.IsDevelopment() ? exception.Message : detail,
                Instance = httpContext.Request.Path
            };

            // Add correlation ID to extensions
            problemDetails.Extensions["correlationId"] = correlationId;
            problemDetails.Extensions["timestamp"] = DateTime.UtcNow;

            // Add stack trace in development
            if (_environment.IsDevelopment())
            {
                problemDetails.Extensions["stackTrace"] = exception.StackTrace;
                problemDetails.Extensions["exceptionType"] = exception.GetType().Name;
            }

            // Set response
            httpContext.Response.StatusCode = statusCode;
            httpContext.Response.ContentType = "application/problem+json";

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true; // Exception was handled
        }

        private static (int statusCode, string title, string detail) MapException(Exception exception)
        {
            return exception switch
            {
                ArgumentNullException => (
                    (int)HttpStatusCode.BadRequest,
                    "Invalid Request",
                    "A required parameter was missing."),

                ArgumentException => (
                    (int)HttpStatusCode.BadRequest,
                    "Invalid Request",
                    "The request contained invalid data."),

                UnauthorizedAccessException => (
                    (int)HttpStatusCode.Unauthorized,
                    "Unauthorized",
                    "You are not authorized to perform this action."),

                KeyNotFoundException => (
                    (int)HttpStatusCode.NotFound,
                    "Not Found",
                    "The requested resource was not found."),

                InvalidOperationException => (
                    (int)HttpStatusCode.Conflict,
                    "Conflict",
                    "The operation could not be completed due to a conflict."),

                NotImplementedException => (
                    (int)HttpStatusCode.NotImplemented,
                    "Not Implemented",
                    "This feature is not yet implemented."),

                OperationCanceledException => (
                    499, // Client Closed Request
                    "Request Cancelled",
                    "The request was cancelled."),

                _ => (
                    (int)HttpStatusCode.InternalServerError,
                    "Internal Server Error",
                    "An unexpected error occurred. Please try again later.")
            };
        }
    }
}


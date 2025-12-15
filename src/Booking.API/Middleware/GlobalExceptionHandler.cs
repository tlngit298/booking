using Booking.API.Models;
using Booking.Domain.Common;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;

namespace Booking.API.Middleware;

/// <summary>
/// Global exception handler that catches unhandled exceptions and transforms them
/// into standardized ApiResponse format with appropriate HTTP status codes.
/// </summary>
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
        _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);

        var (statusCode, response) = exception switch
        {
            ValidationException validationException => HandleValidationException(validationException),
            DomainException domainException => HandleDomainException(domainException),
            _ => HandleGenericException(exception)
        };

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }

    /// <summary>
    /// Handles FluentValidation ValidationException by extracting validation errors
    /// and formatting them into a structured dictionary.
    /// </summary>
    private static (int StatusCode, ApiResponse Response) HandleValidationException(ValidationException exception)
    {
        var validationErrors = exception.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray());

        var response = ApiResponse.ValidationFailureResponse(validationErrors);

        return (StatusCodes.Status400BadRequest, response);
    }

    /// <summary>
    /// Handles domain exceptions as business rule violations.
    /// Returns 400 Bad Request with the domain error message.
    /// </summary>
    private static (int StatusCode, ApiResponse Response) HandleDomainException(DomainException exception)
    {
        var response = ApiResponse.FailureResponse(
            "Domain.RuleViolation",
            exception.Message);

        return (StatusCodes.Status400BadRequest, response);
    }

    /// <summary>
    /// Handles all other unhandled exceptions as internal server errors.
    /// Returns 500 with a generic message (detailed error is logged but not exposed to client).
    /// </summary>
    private static (int StatusCode, ApiResponse Response) HandleGenericException(Exception exception)
    {
        var response = ApiResponse.FailureResponse(
            "Server.InternalError",
            "An internal server error occurred. Please try again later.");

        return (StatusCodes.Status500InternalServerError, response);
    }
}

namespace Booking.Application.Common;

/// <summary>
/// Represents an error with a code and message.
/// </summary>
/// <param name="Code">The error code for programmatic handling.</param>
/// <param name="Message">The human-readable error message.</param>
public sealed record Error(string Code, string Message)
{
    /// <summary>
    /// Represents no error (for successful results).
    /// </summary>
    public static readonly Error None = new(string.Empty, string.Empty);

    /// <summary>
    /// Creates a not found error.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The error message.</param>
    public static Error NotFound(string code, string message) => new(code, message);

    /// <summary>
    /// Creates a validation error.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The error message.</param>
    public static Error Validation(string code, string message) => new(code, message);

    /// <summary>
    /// Creates a conflict error.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The error message.</param>
    public static Error Conflict(string code, string message) => new(code, message);

    /// <summary>
    /// Creates a failure error.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="message">The error message.</param>
    public static Error Failure(string code, string message) => new(code, message);
}

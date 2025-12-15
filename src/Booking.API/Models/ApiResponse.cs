namespace Booking.API.Models;

/// <summary>
/// Represents a standardized API response without a data payload.
/// Used for operations that don't return data (e.g., delete, void updates).
/// </summary>
public class ApiResponse
{
    /// <summary>
    /// Indicates whether the operation was successful.
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// The error message if the operation failed.
    /// </summary>
    public string? Error { get; init; }

    /// <summary>
    /// The error code for programmatic error handling.
    /// </summary>
    public string? ErrorCode { get; init; }

    /// <summary>
    /// Dictionary of validation errors (field name -> array of error messages).
    /// </summary>
    public Dictionary<string, string[]>? ValidationErrors { get; init; }

    /// <summary>
    /// The timestamp when the response was generated.
    /// </summary>
    public DateTime Timestamp { get; init; }

    /// <summary>
    /// Creates a successful response.
    /// </summary>
    public static ApiResponse SuccessResponse() => new()
    {
        Success = true,
        Timestamp = DateTime.UtcNow
    };

    /// <summary>
    /// Creates a failed response with an error.
    /// </summary>
    public static ApiResponse FailureResponse(string errorCode, string errorMessage) => new()
    {
        Success = false,
        ErrorCode = errorCode,
        Error = errorMessage,
        Timestamp = DateTime.UtcNow
    };

    /// <summary>
    /// Creates a validation failure response with multiple validation errors.
    /// </summary>
    public static ApiResponse ValidationFailureResponse(Dictionary<string, string[]> validationErrors) => new()
    {
        Success = false,
        ErrorCode = "Validation.Failed",
        Error = "One or more validation errors occurred",
        ValidationErrors = validationErrors,
        Timestamp = DateTime.UtcNow
    };
}

/// <summary>
/// Represents a standardized API response with a data payload.
/// Used for operations that return data (e.g., queries, create operations).
/// </summary>
/// <typeparam name="T">The type of data being returned.</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// Indicates whether the operation was successful.
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// The data payload if the operation was successful.
    /// </summary>
    public T? Data { get; init; }

    /// <summary>
    /// The error message if the operation failed.
    /// </summary>
    public string? Error { get; init; }

    /// <summary>
    /// The error code for programmatic error handling.
    /// </summary>
    public string? ErrorCode { get; init; }

    /// <summary>
    /// Dictionary of validation errors (field name -> array of error messages).
    /// </summary>
    public Dictionary<string, string[]>? ValidationErrors { get; init; }

    /// <summary>
    /// The timestamp when the response was generated.
    /// </summary>
    public DateTime Timestamp { get; init; }

    /// <summary>
    /// Creates a successful response with data.
    /// </summary>
    public static ApiResponse<T> SuccessResponse(T data) => new()
    {
        Success = true,
        Data = data,
        Timestamp = DateTime.UtcNow
    };

    /// <summary>
    /// Creates a failed response with an error.
    /// </summary>
    public static ApiResponse<T> FailureResponse(string errorCode, string errorMessage) => new()
    {
        Success = false,
        ErrorCode = errorCode,
        Error = errorMessage,
        Timestamp = DateTime.UtcNow
    };

    /// <summary>
    /// Creates a validation failure response with multiple validation errors.
    /// </summary>
    public static ApiResponse<T> ValidationFailureResponse(Dictionary<string, string[]> validationErrors) => new()
    {
        Success = false,
        ErrorCode = "Validation.Failed",
        Error = "One or more validation errors occurred",
        ValidationErrors = validationErrors,
        Timestamp = DateTime.UtcNow
    };
}

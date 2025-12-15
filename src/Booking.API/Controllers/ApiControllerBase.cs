using Booking.API.Models;
using Booking.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace Booking.API.Controllers;

/// <summary>
/// Base controller for all API controllers providing common functionality
/// for mapping Result objects to standardized ApiResponse objects.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>
    /// Maps a Result to an OK response (200) with ApiResponse wrapper.
    /// Returns appropriate error status code if result is a failure.
    /// </summary>
    protected IActionResult OkResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            return Ok(ApiResponse<T>.SuccessResponse(result.Value!));
        }

        return HandleFailure(result.Error);
    }

    /// <summary>
    /// Maps a Result to a Created response (201) with ApiResponse wrapper and location header.
    /// Returns appropriate error status code if result is a failure.
    /// </summary>
    protected IActionResult CreatedResult<T>(Result<T> result, string routeName, object routeValues)
    {
        if (result.IsSuccess)
        {
            return CreatedAtRoute(
                routeName,
                routeValues,
                ApiResponse<T>.SuccessResponse(result.Value!));
        }

        return HandleFailure(result.Error);
    }

    /// <summary>
    /// Maps a Result to a No Content response (204) for successful operations.
    /// Returns appropriate error status code if result is a failure.
    /// </summary>
    protected IActionResult NoContentResult(Result result)
    {
        if (result.IsSuccess)
        {
            return NoContent();
        }

        return HandleFailure(result.Error);
    }

    /// <summary>
    /// Generic result handler that automatically determines the appropriate response
    /// based on whether the result is a success or failure.
    /// </summary>
    protected IActionResult HandleResult<T>(Result<T> result)
    {
        return OkResult(result);
    }

    /// <summary>
    /// Handles failure results by mapping error types to appropriate HTTP status codes
    /// and wrapping them in ApiResponse format.
    /// </summary>
    private IActionResult HandleFailure(Error error)
    {
        var statusCode = GetStatusCode(error);
        var response = ApiResponse.FailureResponse(error.Code, error.Message);

        return StatusCode(statusCode, response);
    }

    /// <summary>
    /// Maps Application layer error codes to HTTP status codes.
    /// </summary>
    private static int GetStatusCode(Error error)
    {
        return error.Code switch
        {
            _ when error.Code.Contains("NotFound") => StatusCodes.Status404NotFound,
            _ when error.Code.Contains("Validation") => StatusCodes.Status400BadRequest,
            _ when error.Code.Contains("Conflict") => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };
    }
}

using Booking.Application.Features.Providers.Commands.ActivateProvider;
using Booking.Application.Features.Providers.Commands.CreateProvider;
using Booking.Application.Features.Providers.Commands.DeactivateProvider;
using Booking.Application.Features.Providers.Commands.UpdateProvider;
using Booking.Application.Features.Providers.Queries.GetProviderById;
using Booking.Application.Features.Providers.Queries.GetProviderBySlug;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Booking.API.Controllers;

/// <summary>
/// Controller for managing provider resources.
/// Demonstrates the ApiResponse pattern with various CQRS operations.
/// </summary>
[Tags("Providers")]
public class ProvidersController : ApiControllerBase
{
    private readonly ISender _sender;

    public ProvidersController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Gets a provider by ID.
    /// </summary>
    /// <param name="id">The provider ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Provider details wrapped in ApiResponse</returns>
    [HttpGet("{id:guid}", Name = nameof(GetProviderById))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProviderById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetProviderByIdQuery(id);
        var result = await _sender.Send(query, cancellationToken);

        return OkResult(result);
    }

    /// <summary>
    /// Gets a provider by slug.
    /// </summary>
    /// <param name="slug">The provider slug</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Provider details wrapped in ApiResponse</returns>
    [HttpGet("by-slug/{slug}", Name = nameof(GetProviderBySlug))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProviderBySlug(
        string slug,
        CancellationToken cancellationToken)
    {
        var query = new GetProviderBySlugQuery(slug);
        var result = await _sender.Send(query, cancellationToken);

        return OkResult(result);
    }

    /// <summary>
    /// Creates a new provider on the Bookity platform.
    /// </summary>
    /// <param name="command">The create provider command containing provider details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created provider ID wrapped in ApiResponse with 201 Created status</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/providers
    ///     {
    ///        "name": "Sunshine Spa",
    ///        "slug": "sunshine-spa",
    ///        "email": "contact@sunshinespa.com",
    ///        "timeZone": "America/New_York",
    ///        "description": "Premium spa and wellness services in downtown Manhattan",
    ///        "phone": "0123456789"
    ///     }
    ///
    /// </remarks>
    /// <response code="201">Provider created successfully - returns the new provider ID</response>
    /// <response code="400">Invalid request - validation errors in the input data</response>
    /// <response code="409">Conflict - a provider with this slug already exists</response>
    /// <response code="500">Internal server error - unexpected error occurred</response>
    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Models.ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Models.ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Models.ApiResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(Models.ApiResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateProvider(
        [FromBody] CreateProviderCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _sender.Send(command, cancellationToken);

        return CreatedResult(
            result,
            nameof(GetProviderById),
            new { id = result.Value });
    }

    /// <summary>
    /// Updates an existing provider.
    /// </summary>
    /// <param name="id">The provider ID</param>
    /// <param name="command">The update provider command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>204 No Content on success, or error response</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateProvider(
        Guid id,
        [FromBody] UpdateProviderCommand command,
        CancellationToken cancellationToken)
    {
        // Ensure the ID in the route matches the command
        if (id != command.ProviderId)
        {
            return BadRequest(Models.ApiResponse.FailureResponse(
                "Provider.IdMismatch",
                "The provider ID in the URL does not match the command"));
        }

        var result = await _sender.Send(command, cancellationToken);

        return NoContentResult(result);
    }

    /// <summary>
    /// Activates a provider.
    /// </summary>
    /// <param name="id">The provider ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>204 No Content on success, or error response</returns>
    [HttpPost("{id:guid}/activate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActivateProvider(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new ActivateProviderCommand(id);
        var result = await _sender.Send(command, cancellationToken);

        return NoContentResult(result);
    }

    /// <summary>
    /// Deactivates a provider.
    /// </summary>
    /// <param name="id">The provider ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>204 No Content on success, or error response</returns>
    [HttpPost("{id:guid}/deactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateProvider(
        Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeactivateProviderCommand(id);
        var result = await _sender.Send(command, cancellationToken);

        return NoContentResult(result);
    }
}

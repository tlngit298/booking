using Booking.Domain.Common;

namespace Booking.Domain.Aggregates.Providers;

/// <summary>
/// Repository interface for Provider aggregate.
/// </summary>
public interface IProviderRepository : IRepository<Provider, ProviderId>
{
    /// <summary>
    /// Gets a provider by their unique slug.
    /// </summary>
    /// <param name="slug">The provider's URL-friendly slug.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The provider if found; otherwise, null.</returns>
    Task<Provider?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a slug already exists.
    /// </summary>
    /// <param name="slug">The slug to check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the slug exists; otherwise, false.</returns>
    Task<bool> SlugExistsAsync(string slug, CancellationToken cancellationToken = default);
}

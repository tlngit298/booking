using Booking.Domain.Aggregates.Providers;
using Booking.Domain.Common;

namespace Booking.Domain.Aggregates.Services;

/// <summary>
/// Repository interface for Service aggregate.
/// </summary>
public interface IServiceRepository : IRepository<Service, ServiceId>
{
    /// <summary>
    /// Gets all services for a specific provider.
    /// </summary>
    /// <param name="providerId">The provider's identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A read-only list of services.</returns>
    Task<IReadOnlyList<Service>> GetByProviderIdAsync(
        ProviderId providerId,
        CancellationToken cancellationToken = default);
}

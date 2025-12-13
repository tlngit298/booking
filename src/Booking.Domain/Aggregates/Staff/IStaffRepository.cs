using Booking.Domain.Aggregates.Providers;
using Booking.Domain.Aggregates.Services;
using Booking.Domain.Common;

namespace Booking.Domain.Aggregates.Staff;

/// <summary>
/// Repository interface for Staff aggregate.
/// </summary>
public interface IStaffRepository : IRepository<Staff, StaffId>
{
    /// <summary>
    /// Gets all staff members for a specific provider.
    /// </summary>
    /// <param name="providerId">The provider's identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A read-only list of staff members.</returns>
    Task<IReadOnlyList<Staff>> GetByProviderIdAsync(
        ProviderId providerId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all staff members who can perform a specific service.
    /// </summary>
    /// <param name="serviceId">The service identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A read-only list of staff members assigned to the service.</returns>
    Task<IReadOnlyList<Staff>> GetByServiceIdAsync(
        ServiceId serviceId,
        CancellationToken cancellationToken = default);
}

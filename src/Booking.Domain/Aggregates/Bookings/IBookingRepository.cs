using Booking.Domain.Aggregates.Services;
using Booking.Domain.Aggregates.Staff;
using Booking.Domain.Aggregates.Customers;
using Booking.Domain.Common;

namespace Booking.Domain.Aggregates.Bookings;

/// <summary>
/// Repository interface for Booking aggregate.
/// </summary>
public interface IBookingRepository : IRepository<Booking, BookingId>
{
    /// <summary>
    /// Gets a booking by its unique booking number.
    /// </summary>
    /// <param name="bookingNumber">The booking number (e.g., "BK-2025-001234").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The booking if found; otherwise, null.</returns>
    Task<Booking?> GetByBookingNumberAsync(
        string bookingNumber,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all bookings for a specific service on a specific date.
    /// Critical for checking service availability in Direct mode.
    /// </summary>
    /// <param name="serviceId">The service identifier.</param>
    /// <param name="date">The booking date.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A read-only list of bookings.</returns>
    Task<IReadOnlyList<Booking>> GetByServiceAndDateAsync(
        ServiceId serviceId,
        DateOnly date,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all bookings for a specific staff member on a specific date.
    /// Critical for checking staff availability in StaffBased mode.
    /// </summary>
    /// <param name="staffId">The staff identifier.</param>
    /// <param name="date">The booking date.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A read-only list of bookings.</returns>
    Task<IReadOnlyList<Booking>> GetByStaffAndDateAsync(
        StaffId staffId,
        DateOnly date,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all bookings for a specific customer.
    /// Used for displaying customer booking history.
    /// </summary>
    /// <param name="customerId">The customer identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A read-only list of bookings.</returns>
    Task<IReadOnlyList<Booking>> GetByCustomerIdAsync(
        CustomerId customerId,
        CancellationToken cancellationToken = default);
}

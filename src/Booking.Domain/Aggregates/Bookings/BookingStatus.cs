namespace Booking.Domain.Aggregates.Bookings;

/// <summary>
/// Defines the status of a booking throughout its lifecycle.
/// </summary>
public enum BookingStatus
{
    /// <summary>
    /// Booking has been created but not yet confirmed.
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Booking has been confirmed by the tenant/system.
    /// </summary>
    Confirmed = 2,

    /// <summary>
    /// Booking has been cancelled by customer or tenant.
    /// </summary>
    Cancelled = 3,

    /// <summary>
    /// Booking has been completed successfully.
    /// </summary>
    Completed = 4,

    /// <summary>
    /// Customer did not show up for the booking.
    /// </summary>
    NoShow = 5
}

namespace Booking.Domain.Aggregates.Services;

/// <summary>
/// Defines the booking mode for a service.
/// </summary>
public enum BookingMode
{
    /// <summary>
    /// Book service directly without staff selection.
    /// Used for: courts, meeting rooms, solo consultants.
    /// Schedule is on the Service itself.
    /// </summary>
    Direct = 1,

    /// <summary>
    /// Book service with specific staff member selection.
    /// Used for: spas, salons, clinics.
    /// Schedule is on the Staff, not the Service.
    /// </summary>
    StaffBased = 2
}

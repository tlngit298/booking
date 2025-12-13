namespace Booking.Domain.Aggregates.Bookings;

/// <summary>
/// Represents a unique identifier for a Booking aggregate.
/// </summary>
public readonly record struct BookingId(Guid Value)
{
    /// <summary>
    /// Creates a new BookingId with a generated GUID.
    /// </summary>
    public static BookingId New() => new(Guid.NewGuid());
}

namespace Booking.Domain.Aggregates.Staff;

/// <summary>
/// Represents a unique identifier for a Staff aggregate.
/// </summary>
public readonly record struct StaffId(Guid Value)
{
    /// <summary>
    /// Creates a new StaffId with a generated GUID.
    /// </summary>
    public static StaffId New() => new(Guid.NewGuid());
}

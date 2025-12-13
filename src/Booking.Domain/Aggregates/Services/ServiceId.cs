namespace Booking.Domain.Aggregates.Services;

/// <summary>
/// Represents a unique identifier for a Service aggregate.
/// </summary>
public readonly record struct ServiceId(Guid Value)
{
    /// <summary>
    /// Creates a new ServiceId with a generated GUID.
    /// </summary>
    public static ServiceId New() => new(Guid.NewGuid());
}

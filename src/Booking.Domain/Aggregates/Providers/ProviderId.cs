namespace Booking.Domain.Aggregates.Providers;

/// <summary>
/// Represents a unique identifier for a Provider aggregate.
/// </summary>
public readonly record struct ProviderId(Guid Value)
{
    /// <summary>
    /// Creates a new ProviderId with a generated GUID.
    /// </summary>
    public static ProviderId New() => new(Guid.NewGuid());
}

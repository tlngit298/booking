namespace Booking.Domain.Aggregates.Customers;

/// <summary>
/// Represents a unique identifier for a Customer aggregate.
/// </summary>
public readonly record struct CustomerId(Guid Value)
{
    /// <summary>
    /// Creates a new CustomerId with a generated GUID.
    /// </summary>
    public static CustomerId New() => new(Guid.NewGuid());
}

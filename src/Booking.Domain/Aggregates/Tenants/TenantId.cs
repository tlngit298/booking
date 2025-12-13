namespace Booking.Domain.Aggregates.Tenants;

/// <summary>
/// Represents a unique identifier for a Tenant aggregate.
/// </summary>
public readonly record struct TenantId(Guid Value)
{
    /// <summary>
    /// Creates a new TenantId with a generated GUID.
    /// </summary>
    public static TenantId New() => new(Guid.NewGuid());
}

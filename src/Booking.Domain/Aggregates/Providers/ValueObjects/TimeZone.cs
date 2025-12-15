using Booking.Domain.Common;

namespace Booking.Domain.Aggregates.Providers.ValueObjects;

/// <summary>
/// Value object representing an IANA timezone identifier.
/// Ensures the timezone is valid and recognized by the system.
/// </summary>
public sealed record TimeZone : ValueObject<string>
{
    private TimeZone(string value) : base(value)
    {
    }

    /// <summary>
    /// Creates a new TimeZone value object.
    /// </summary>
    /// <param name="value">The IANA timezone identifier (e.g., "America/New_York").</param>
    /// <returns>A new TimeZone instance.</returns>
    /// <exception cref="DomainException">Thrown when validation fails.</exception>
    public static TimeZone Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("TimeZone is required");

        try
        {
            TimeZoneInfo.FindSystemTimeZoneById(value);
        }
        catch (Exception)
        {
            throw new DomainException($"Invalid IANA timezone: {value}");
        }

        return new TimeZone(value);
    }
}

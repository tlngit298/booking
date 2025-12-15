using Booking.Domain.Common;

namespace Booking.Domain.Aggregates.Providers.ValueObjects;

/// <summary>
/// Value object representing a provider's business name.
/// Ensures the name is not empty and does not exceed maximum length.
/// </summary>
public sealed record ProviderName : ValueObject<string>
{
    private const int MaxLength = 200;

    private ProviderName(string value) : base(value)
    {
    }

    /// <summary>
    /// Creates a new ProviderName value object.
    /// </summary>
    /// <param name="value">The provider name.</param>
    /// <returns>A new ProviderName instance.</returns>
    /// <exception cref="DomainException">Thrown when validation fails.</exception>
    public static ProviderName Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Provider name is required");

        if (value.Length > MaxLength)
            throw new DomainException($"Provider name must not exceed {MaxLength} characters");

        return new ProviderName(value);
    }
}

using Booking.Domain.Common;

namespace Booking.Domain.Aggregates.Providers.ValueObjects;

/// <summary>
/// Value object representing an email address.
/// Ensures the email is in valid format and does not exceed maximum length.
/// </summary>
public sealed record Email : ValueObject<string>
{
    private const int MaxLength = 255;

    private Email(string value) : base(value)
    {
    }

    /// <summary>
    /// Creates a new Email value object.
    /// </summary>
    /// <param name="value">The email address.</param>
    /// <returns>A new Email instance.</returns>
    /// <exception cref="DomainException">Thrown when validation fails.</exception>
    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Email is required");

        if (value.Length > MaxLength)
            throw new DomainException($"Email must not exceed {MaxLength} characters");

        // Simple email validation - domain rule: must be valid email format
        if (!value.Contains('@') || !value.Contains('.'))
            throw new DomainException("Email must be in valid format");

        return new Email(value);
    }
}

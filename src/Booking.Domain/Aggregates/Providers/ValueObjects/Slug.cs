using System.Text.RegularExpressions;
using Booking.Domain.Common;

namespace Booking.Domain.Aggregates.Providers.ValueObjects;

/// <summary>
/// Value object representing a URL-friendly slug.
/// Ensures the slug contains only lowercase letters, numbers, and hyphens.
/// </summary>
public sealed record Slug : ValueObject<string>
{
    private const int MaxLength = 100;
    private static readonly Regex SlugPattern = new("^[a-z0-9-]+$", RegexOptions.Compiled);

    private Slug(string value) : base(value)
    {
    }

    /// <summary>
    /// Creates a new Slug value object.
    /// </summary>
    /// <param name="value">The slug value.</param>
    /// <returns>A new Slug instance.</returns>
    /// <exception cref="DomainException">Thrown when validation fails.</exception>
    public static Slug Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Slug is required");

        if (value.Length > MaxLength)
            throw new DomainException($"Slug must not exceed {MaxLength} characters");

        if (!SlugPattern.IsMatch(value))
            throw new DomainException("Slug must contain only lowercase letters, numbers, and hyphens");

        return new Slug(value);
    }
}

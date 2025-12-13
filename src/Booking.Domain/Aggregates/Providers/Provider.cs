using Booking.Domain.Common;

namespace Booking.Domain.Aggregates.Providers;

/// <summary>
/// Represents a business owner/provider in the booking system.
/// Examples: badminton yard, spa, meeting room provider.
/// </summary>
public sealed class Provider : AggregateRoot<ProviderId>
{
    /// <summary>
    /// Gets the provider's business name.
    /// </summary>
    public string Name { get; private set; } = null!;

    /// <summary>
    /// Gets the URL-friendly unique identifier for the provider.
    /// </summary>
    public string Slug { get; private set; } = null!;

    /// <summary>
    /// Gets the provider's business description.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Gets the provider's contact email.
    /// </summary>
    public string Email { get; private set; } = null!;

    /// <summary>
    /// Gets the provider's contact phone number.
    /// </summary>
    public string? Phone { get; private set; }

    /// <summary>
    /// Gets the provider's timezone (IANA format, e.g., "Asia/Ho_Chi_Minh").
    /// </summary>
    public string TimeZone { get; private set; } = null!;

    /// <summary>
    /// Gets whether the provider is active.
    /// </summary>
    public bool IsActive { get; private set; }

    // Private constructor for EF Core
    private Provider() { }

    /// <summary>
    /// Creates a new provider.
    /// </summary>
    /// <param name="name">The provider's business name.</param>
    /// <param name="slug">The URL-friendly unique identifier.</param>
    /// <param name="email">The provider's contact email.</param>
    /// <param name="timeZone">The provider's timezone (IANA format).</param>
    /// <returns>A new Provider instance.</returns>
    /// <exception cref="DomainException">Thrown when validation fails.</exception>
    public static Provider Create(
        string name,
        string slug,
        string email,
        string timeZone)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Provider name is required");

        if (string.IsNullOrWhiteSpace(slug))
            throw new DomainException("Provider slug is required");

        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Provider email is required");

        if (string.IsNullOrWhiteSpace(timeZone))
            throw new DomainException("Provider timezone is required");

        var provider = new Provider
        {
            Id = ProviderId.New(),
            Name = name,
            Slug = slug,
            Email = email,
            TimeZone = timeZone,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Domain event will be added in future implementation
        // provider.AddDomainEvent(new ProviderCreatedEvent(provider.Id, name, slug));

        return provider;
    }

    /// <summary>
    /// Updates the provider's information.
    /// </summary>
    /// <param name="name">The updated business name.</param>
    /// <param name="description">The updated business description.</param>
    /// <param name="email">The updated contact email.</param>
    /// <param name="phone">The updated contact phone.</param>
    /// <exception cref="DomainException">Thrown when validation fails.</exception>
    public void Update(
        string name,
        string? description,
        string email,
        string? phone)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Provider name is required");

        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Provider email is required");

        Name = name;
        Description = description;
        Email = email;
        Phone = phone;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Activates the provider.
    /// </summary>
    /// <exception cref="DomainException">Thrown when the provider is already active.</exception>
    public void Activate()
    {
        if (IsActive)
            throw new DomainException("Provider is already active");

        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Deactivates the provider.
    /// </summary>
    /// <exception cref="DomainException">Thrown when the provider is already inactive.</exception>
    public void Deactivate()
    {
        if (!IsActive)
            throw new DomainException("Provider is already inactive");

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}

using Booking.Domain.Common;
using Booking.Domain.Aggregates.Providers.ValueObjects;

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
    public ProviderName Name { get; private set; } = null!;

    /// <summary>
    /// Gets the URL-friendly unique identifier for the provider.
    /// </summary>
    public Slug Slug { get; private set; } = null!;

    /// <summary>
    /// Gets the provider's business description.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Gets the provider's contact email.
    /// </summary>
    public Email Email { get; private set; } = null!;

    /// <summary>
    /// Gets the provider's contact phone number.
    /// </summary>
    public string? Phone { get; private set; }

    /// <summary>
    /// Gets the provider's timezone (IANA format, e.g., "Asia/Ho_Chi_Minh").
    /// </summary>
    public ValueObjects.TimeZone TimeZone { get; private set; } = null!;

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
        var provider = new Provider
        {
            Id = ProviderId.New(),
            Name = ProviderName.Create(name),
            Slug = Slug.Create(slug),
            Email = Email.Create(email),
            TimeZone = ValueObjects.TimeZone.Create(timeZone),
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Domain event will be added in future implementation
        // provider.AddDomainEvent(new ProviderCreatedEvent(provider.Id, name, slug));

        return provider;
    }

    /// <summary>
    /// Creates a new provider with optional fields.
    /// </summary>
    /// <param name="name">The provider's business name.</param>
    /// <param name="slug">The URL-friendly unique identifier.</param>
    /// <param name="email">The provider's contact email.</param>
    /// <param name="timeZone">The provider's timezone (IANA format).</param>
    /// <param name="description">The provider's business description (optional).</param>
    /// <param name="phone">The provider's contact phone (optional).</param>
    /// <returns>A new Provider instance.</returns>
    /// <exception cref="DomainException">Thrown when validation fails.</exception>
    public static Provider Create(
        string name,
        string slug,
        string email,
        string timeZone,
        string? description = null,
        string? phone = null)
    {
        // Validate optional fields
        if (!string.IsNullOrWhiteSpace(description) && description.Length > 2000)
            throw new DomainException("Description must not exceed 2000 characters");

        if (!string.IsNullOrWhiteSpace(phone) && phone.Length > 20)
            throw new DomainException("Phone must not exceed 20 characters");

        var provider = new Provider
        {
            Id = ProviderId.New(),
            Name = ProviderName.Create(name),
            Slug = Slug.Create(slug),
            Email = Email.Create(email),
            TimeZone = ValueObjects.TimeZone.Create(timeZone),
            Description = description,
            Phone = phone,
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
        // Validate optional fields
        if (!string.IsNullOrWhiteSpace(description) && description.Length > 1000)
            throw new DomainException("Description must not exceed 2000 characters");

        if (!string.IsNullOrWhiteSpace(phone) && phone.Length > 20)
            throw new DomainException("Phone must not exceed 20 characters");

        Name = ProviderName.Create(name);
        Description = description;
        Email = Email.Create(email);
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

using Booking.Domain.Common;

namespace Booking.Domain.Aggregates.Providers.Events;

/// <summary>
/// Domain event raised when a new provider is created.
/// Contains complete provider data snapshot for event handlers.
/// </summary>
public sealed record ProviderCreatedEvent : DomainEvent
{
    /// <summary>
    /// Gets the unique identifier of the created provider.
    /// </summary>
    public ProviderId ProviderId { get; init; }

    /// <summary>
    /// Gets the provider's business name.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// Gets the URL-friendly unique identifier.
    /// </summary>
    public string Slug { get; init; }

    /// <summary>
    /// Gets the provider's contact email.
    /// </summary>
    public string Email { get; init; }

    /// <summary>
    /// Gets the provider's timezone (IANA format).
    /// </summary>
    public string TimeZone { get; init; }

    /// <summary>
    /// Gets the provider's business description (optional).
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Gets the provider's contact phone (optional).
    /// </summary>
    public string? Phone { get; init; }

    /// <summary>
    /// Creates a new ProviderCreatedEvent with all required data.
    /// </summary>
    public ProviderCreatedEvent(
        ProviderId providerId,
        string name,
        string slug,
        string email,
        string timeZone,
        string? description = null,
        string? phone = null)
    {
        ProviderId = providerId;
        Name = name;
        Slug = slug;
        Email = email;
        TimeZone = timeZone;
        Description = description;
        Phone = phone;
    }
}

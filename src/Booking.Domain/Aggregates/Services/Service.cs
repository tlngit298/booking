using Booking.Domain.Aggregates.Providers;
using Booking.Domain.Common;
using Booking.Domain.ValueObjects;

namespace Booking.Domain.Aggregates.Services;

/// <summary>
/// Represents a bookable service offering in the system.
/// Examples: badminton court, massage service, meeting room, consultation.
/// </summary>
public sealed class Service : AggregateRoot<ServiceId>
{
    /// <summary>
    /// Gets the identifier of the provider offering this service.
    /// </summary>
    public ProviderId ProviderId { get; private set; }

    /// <summary>
    /// Gets the service name.
    /// </summary>
    public string Name { get; private set; } = null!;

    /// <summary>
    /// Gets the service description.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Gets the service duration in minutes.
    /// </summary>
    public int DurationMinutes { get; private set; }

    /// <summary>
    /// Gets the service price.
    /// </summary>
    public decimal Price { get; private set; }

    /// <summary>
    /// Gets the currency code (ISO 4217 format, e.g., "USD", "VND").
    /// </summary>
    public string Currency { get; private set; } = null!;

    /// <summary>
    /// Gets the booking mode for this service.
    /// Direct mode: book the service directly (courts, rooms).
    /// StaffBased mode: must select staff member (spa, salon).
    /// </summary>
    public BookingMode BookingMode { get; private set; }

    /// <summary>
    /// Gets the weekly schedule for this service.
    /// Only applicable for Direct mode services. Null for StaffBased mode.
    /// </summary>
    public WeeklySchedule? Schedule { get; private set; }

    /// <summary>
    /// Gets the maximum number of concurrent bookings allowed.
    /// Used for group classes or multiple identical resources.
    /// </summary>
    public int MaxConcurrentBookings { get; private set; }

    /// <summary>
    /// Gets whether the service is active and available for booking.
    /// </summary>
    public bool IsActive { get; private set; }

    // Private constructor for EF Core
    private Service() { }

    /// <summary>
    /// Creates a new service.
    /// </summary>
    /// <param name="providerId">The provider offering this service.</param>
    /// <param name="name">The service name.</param>
    /// <param name="durationMinutes">The service duration in minutes.</param>
    /// <param name="price">The service price.</param>
    /// <param name="currency">The currency code (ISO 4217).</param>
    /// <param name="bookingMode">The booking mode (Direct or StaffBased).</param>
    /// <returns>A new Service instance.</returns>
    /// <exception cref="DomainException">Thrown when validation fails.</exception>
    public static Service Create(
        ProviderId providerId,
        string name,
        int durationMinutes,
        decimal price,
        string currency,
        BookingMode bookingMode)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Service name is required");

        if (durationMinutes <= 0)
            throw new DomainException("Service duration must be greater than zero");

        if (price < 0)
            throw new DomainException("Service price cannot be negative");

        if (string.IsNullOrWhiteSpace(currency))
            throw new DomainException("Service currency is required");

        var service = new Service
        {
            Id = ServiceId.New(),
            ProviderId = providerId,
            Name = name,
            DurationMinutes = durationMinutes,
            Price = price,
            Currency = currency,
            BookingMode = bookingMode,
            MaxConcurrentBookings = 1, // Default to exclusive booking
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Domain event will be added in future implementation
        // service.AddDomainEvent(new ServiceCreatedEvent(service.Id, providerId, name, bookingMode));

        return service;
    }

    /// <summary>
    /// Updates the service information.
    /// </summary>
    /// <param name="name">The updated service name.</param>
    /// <param name="description">The updated service description.</param>
    /// <param name="durationMinutes">The updated service duration in minutes.</param>
    /// <param name="price">The updated service price.</param>
    /// <exception cref="DomainException">Thrown when validation fails.</exception>
    public void Update(
        string name,
        string? description,
        int durationMinutes,
        decimal price)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Service name is required");

        if (durationMinutes <= 0)
            throw new DomainException("Service duration must be greater than zero");

        if (price < 0)
            throw new DomainException("Service price cannot be negative");

        Name = name;
        Description = description;
        DurationMinutes = durationMinutes;
        Price = price;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Sets the weekly schedule for this service.
    /// Only allowed for Direct mode services.
    /// </summary>
    /// <param name="schedule">The weekly schedule.</param>
    /// <exception cref="DomainException">Thrown when called on a StaffBased service.</exception>
    public void SetSchedule(WeeklySchedule schedule)
    {
        if (BookingMode == BookingMode.StaffBased)
            throw new DomainException("Cannot set schedule for staff-based services");

        Schedule = schedule ?? throw new ArgumentNullException(nameof(schedule));
        UpdatedAt = DateTime.UtcNow;

        // Domain event will be added in future implementation
        // AddDomainEvent(new ServiceScheduleUpdatedEvent(Id, DateTime.UtcNow));
    }

    /// <summary>
    /// Sets the maximum number of concurrent bookings allowed.
    /// </summary>
    /// <param name="max">The maximum concurrent bookings (must be at least 1).</param>
    /// <exception cref="DomainException">Thrown when max is less than 1.</exception>
    public void SetMaxConcurrentBookings(int max)
    {
        if (max < 1)
            throw new DomainException("Max concurrent bookings must be at least 1");

        MaxConcurrentBookings = max;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Checks if this service requires staff selection during booking.
    /// </summary>
    /// <returns>True if BookingMode is StaffBased; otherwise, false.</returns>
    public bool RequiresStaff() => BookingMode == BookingMode.StaffBased;

    /// <summary>
    /// Activates the service, making it available for booking.
    /// </summary>
    /// <exception cref="DomainException">Thrown when the service is already active.</exception>
    public void Activate()
    {
        if (IsActive)
            throw new DomainException("Service is already active");

        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Deactivates the service, making it unavailable for booking.
    /// </summary>
    /// <exception cref="DomainException">Thrown when the service is already inactive.</exception>
    public void Deactivate()
    {
        if (!IsActive)
            throw new DomainException("Service is already inactive");

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}

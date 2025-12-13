using Booking.Domain.Aggregates.Providers;
using Booking.Domain.Aggregates.Services;
using Booking.Domain.Common;
using Booking.Domain.ValueObjects;

namespace Booking.Domain.Aggregates.Staff;

/// <summary>
/// Represents a staff member who delivers StaffBased services.
/// Examples: spa therapist, salon stylist, clinic doctor.
/// Only exists for services with BookingMode.StaffBased.
/// </summary>
public sealed class Staff : AggregateRoot<StaffId>
{
    private readonly List<ServiceId> _serviceIds = new();

    /// <summary>
    /// Gets the identifier of the provider employing this staff member.
    /// </summary>
    public ProviderId ProviderId { get; private set; }

    /// <summary>
    /// Gets the staff member's name.
    /// </summary>
    public string Name { get; private set; } = null!;

    /// <summary>
    /// Gets the staff member's email address.
    /// </summary>
    public string? Email { get; private set; }

    /// <summary>
    /// Gets the staff member's phone number.
    /// </summary>
    public string? Phone { get; private set; }

    /// <summary>
    /// Gets the staff member's weekly working schedule.
    /// </summary>
    public WeeklySchedule Schedule { get; private set; } = null!;

    /// <summary>
    /// Gets the read-only list of service IDs this staff member can perform.
    /// </summary>
    public IReadOnlyList<ServiceId> ServiceIds => _serviceIds.AsReadOnly();

    /// <summary>
    /// Gets whether the staff member is active and available for bookings.
    /// </summary>
    public bool IsActive { get; private set; }

    // Private constructor for EF Core
    private Staff() { }

    /// <summary>
    /// Creates a new staff member.
    /// </summary>
    /// <param name="providerId">The provider employing this staff member.</param>
    /// <param name="name">The staff member's name.</param>
    /// <returns>A new Staff instance.</returns>
    /// <exception cref="DomainException">Thrown when validation fails.</exception>
    public static Staff Create(ProviderId providerId, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Staff name is required");

        var staff = new Staff
        {
            Id = StaffId.New(),
            ProviderId = providerId,
            Name = name,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Domain event will be added in future implementation
        // staff.AddDomainEvent(new StaffCreatedEvent(staff.Id, providerId, name));

        return staff;
    }

    /// <summary>
    /// Updates the staff member's information.
    /// </summary>
    /// <param name="name">The updated name.</param>
    /// <param name="email">The updated email address.</param>
    /// <param name="phone">The updated phone number.</param>
    /// <exception cref="DomainException">Thrown when validation fails.</exception>
    public void Update(string name, string? email, string? phone)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Staff name is required");

        Name = name;
        Email = email;
        Phone = phone;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Sets the weekly schedule for this staff member.
    /// </summary>
    /// <param name="schedule">The weekly schedule.</param>
    /// <exception cref="ArgumentNullException">Thrown when schedule is null.</exception>
    public void SetSchedule(WeeklySchedule schedule)
    {
        Schedule = schedule ?? throw new ArgumentNullException(nameof(schedule));
        UpdatedAt = DateTime.UtcNow;

        // Domain event will be added in future implementation
        // AddDomainEvent(new StaffScheduleUpdatedEvent(Id, DateTime.UtcNow));
    }

    /// <summary>
    /// Assigns this staff member to a service.
    /// </summary>
    /// <param name="serviceId">The service identifier.</param>
    public void AssignService(ServiceId serviceId)
    {
        if (!_serviceIds.Contains(serviceId))
        {
            _serviceIds.Add(serviceId);
            UpdatedAt = DateTime.UtcNow;

            // Domain event will be added in future implementation
            // AddDomainEvent(new StaffServiceAssignedEvent(Id, serviceId));
        }
    }

    /// <summary>
    /// Unassigns this staff member from a service.
    /// </summary>
    /// <param name="serviceId">The service identifier.</param>
    public void UnassignService(ServiceId serviceId)
    {
        if (_serviceIds.Remove(serviceId))
        {
            UpdatedAt = DateTime.UtcNow;

            // Domain event will be added in future implementation
            // AddDomainEvent(new StaffServiceUnassignedEvent(Id, serviceId));
        }
    }

    /// <summary>
    /// Checks if this staff member is available at a specific day and time.
    /// This only checks the schedule, not actual bookings.
    /// </summary>
    /// <param name="dayOfWeek">The day of the week.</param>
    /// <param name="time">The time to check.</param>
    /// <returns>True if the staff member's schedule includes the specified day and time; otherwise, false.</returns>
    public bool IsAvailableAt(DayOfWeek dayOfWeek, TimeOnly time)
    {
        if (Schedule == null)
            return false;

        if (!Schedule.IsWorkingDay(dayOfWeek))
            return false;

        var hours = Schedule.GetHours(dayOfWeek);
        return hours?.Contains(time) ?? false;
    }

    /// <summary>
    /// Activates the staff member, making them available for bookings.
    /// </summary>
    /// <exception cref="DomainException">Thrown when the staff member is already active.</exception>
    public void Activate()
    {
        if (IsActive)
            throw new DomainException("Staff member is already active");

        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Deactivates the staff member, making them unavailable for bookings.
    /// </summary>
    /// <exception cref="DomainException">Thrown when the staff member is already inactive.</exception>
    public void Deactivate()
    {
        if (!IsActive)
            throw new DomainException("Staff member is already inactive");

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}

using Booking.Domain.Aggregates.Providers;
using Booking.Domain.Aggregates.Services;
using Booking.Domain.Aggregates.Staff;
using Booking.Domain.Aggregates.Customers;
using Booking.Domain.Common;

namespace Booking.Domain.Aggregates.Bookings;

/// <summary>
/// Represents a booking transaction that ties together a customer, service, and optionally staff.
/// Supports both Direct mode (courts, rooms) and StaffBased mode (spa, salon) bookings.
/// </summary>
public sealed class Booking : AggregateRoot<BookingId>
{
    /// <summary>
    /// Gets the user-friendly booking number (e.g., "BK-2025-001234").
    /// </summary>
    public string BookingNumber { get; private set; } = null!;

    /// <summary>
    /// Gets the identifier of the provider offering the service.
    /// </summary>
    public ProviderId ProviderId { get; private set; }

    /// <summary>
    /// Gets the identifier of the booked service.
    /// </summary>
    public ServiceId ServiceId { get; private set; }

    /// <summary>
    /// Gets the identifier of the staff member (null for Direct mode bookings).
    /// </summary>
    public StaffId? StaffId { get; private set; }

    /// <summary>
    /// Gets the identifier of the customer making the booking.
    /// </summary>
    public CustomerId CustomerId { get; private set; }

    /// <summary>
    /// Gets the date of the booking.
    /// </summary>
    public DateOnly Date { get; private set; }

    /// <summary>
    /// Gets the start time of the booking.
    /// </summary>
    public TimeOnly StartTime { get; private set; }

    /// <summary>
    /// Gets the end time of the booking.
    /// </summary>
    public TimeOnly EndTime { get; private set; }

    /// <summary>
    /// Gets the service name at the time of booking (snapshot for historical accuracy).
    /// </summary>
    public string ServiceName { get; private set; } = null!;

    /// <summary>
    /// Gets the service price at the time of booking (snapshot for historical accuracy).
    /// </summary>
    public decimal ServicePrice { get; private set; }

    /// <summary>
    /// Gets the service currency at the time of booking (snapshot for historical accuracy).
    /// </summary>
    public string ServiceCurrency { get; private set; } = null!;

    /// <summary>
    /// Gets the staff name at the time of booking (snapshot, null for Direct mode).
    /// </summary>
    public string? StaffName { get; private set; }

    /// <summary>
    /// Gets the current status of the booking.
    /// </summary>
    public BookingStatus Status { get; private set; }

    /// <summary>
    /// Gets any notes provided by the customer.
    /// </summary>
    public string? CustomerNotes { get; private set; }

    /// <summary>
    /// Gets the reason for cancellation (if cancelled).
    /// </summary>
    public string? CancellationReason { get; private set; }

    // Private constructor for EF Core
    private Booking() { }

    /// <summary>
    /// Creates a new direct booking (without staff assignment).
    /// Use this for Direct mode services like courts, rooms, or solo consultants.
    /// </summary>
    /// <param name="bookingNumber">The user-friendly booking number.</param>
    /// <param name="providerId">The provider offering the service.</param>
    /// <param name="serviceId">The service being booked.</param>
    /// <param name="customerId">The customer making the booking.</param>
    /// <param name="date">The date of the booking.</param>
    /// <param name="startTime">The start time.</param>
    /// <param name="endTime">The end time.</param>
    /// <param name="serviceName">The service name (snapshot).</param>
    /// <param name="servicePrice">The service price (snapshot).</param>
    /// <param name="serviceCurrency">The service currency (snapshot).</param>
    /// <param name="customerNotes">Optional customer notes.</param>
    /// <returns>A new Booking instance in Pending status.</returns>
    /// <exception cref="DomainException">Thrown when validation fails.</exception>
    public static Booking CreateDirect(
        string bookingNumber,
        ProviderId providerId,
        ServiceId serviceId,
        CustomerId customerId,
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        string serviceName,
        decimal servicePrice,
        string serviceCurrency,
        string? customerNotes)
    {
        // Validate required fields
        if (string.IsNullOrWhiteSpace(bookingNumber))
            throw new DomainException("Booking number is required");

        if (endTime <= startTime)
            throw new DomainException("End time must be after start time");

        if (string.IsNullOrWhiteSpace(serviceName))
            throw new DomainException("Service name is required");

        if (servicePrice < 0)
            throw new DomainException("Service price cannot be negative");

        if (string.IsNullOrWhiteSpace(serviceCurrency))
            throw new DomainException("Service currency is required");

        var booking = new Booking
        {
            Id = BookingId.New(),
            BookingNumber = bookingNumber,
            ProviderId = providerId,
            ServiceId = serviceId,
            StaffId = null,  // Direct mode: no staff
            CustomerId = customerId,
            Date = date,
            StartTime = startTime,
            EndTime = endTime,
            ServiceName = serviceName,
            ServicePrice = servicePrice,
            ServiceCurrency = serviceCurrency,
            StaffName = null,  // Direct mode: no staff name
            Status = BookingStatus.Pending,
            CustomerNotes = customerNotes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Domain event will be added in future implementation
        // booking.AddDomainEvent(new BookingCreatedEvent(booking.Id, bookingNumber, serviceId, null, customerId, date, startTime));

        return booking;
    }

    /// <summary>
    /// Creates a new staff-based booking (with staff assignment).
    /// Use this for StaffBased mode services like spa, salon, or clinic treatments.
    /// </summary>
    /// <param name="bookingNumber">The user-friendly booking number.</param>
    /// <param name="providerId">The provider offering the service.</param>
    /// <param name="serviceId">The service being booked.</param>
    /// <param name="staffId">The staff member providing the service.</param>
    /// <param name="customerId">The customer making the booking.</param>
    /// <param name="date">The date of the booking.</param>
    /// <param name="startTime">The start time.</param>
    /// <param name="endTime">The end time.</param>
    /// <param name="serviceName">The service name (snapshot).</param>
    /// <param name="servicePrice">The service price (snapshot).</param>
    /// <param name="serviceCurrency">The service currency (snapshot).</param>
    /// <param name="staffName">The staff member's name (snapshot).</param>
    /// <param name="customerNotes">Optional customer notes.</param>
    /// <returns>A new Booking instance in Pending status.</returns>
    /// <exception cref="DomainException">Thrown when validation fails.</exception>
    public static Booking CreateWithStaff(
        string bookingNumber,
        ProviderId providerId,
        ServiceId serviceId,
        StaffId staffId,
        CustomerId customerId,
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        string serviceName,
        decimal servicePrice,
        string serviceCurrency,
        string staffName,
        string? customerNotes)
    {
        // Validate required fields
        if (string.IsNullOrWhiteSpace(bookingNumber))
            throw new DomainException("Booking number is required");

        if (endTime <= startTime)
            throw new DomainException("End time must be after start time");

        if (string.IsNullOrWhiteSpace(serviceName))
            throw new DomainException("Service name is required");

        if (servicePrice < 0)
            throw new DomainException("Service price cannot be negative");

        if (string.IsNullOrWhiteSpace(serviceCurrency))
            throw new DomainException("Service currency is required");

        if (string.IsNullOrWhiteSpace(staffName))
            throw new DomainException("Staff name is required for staff-based bookings");

        var booking = new Booking
        {
            Id = BookingId.New(),
            BookingNumber = bookingNumber,
            ProviderId = providerId,
            ServiceId = serviceId,
            StaffId = staffId,  // StaffBased mode: staff required
            CustomerId = customerId,
            Date = date,
            StartTime = startTime,
            EndTime = endTime,
            ServiceName = serviceName,
            ServicePrice = servicePrice,
            ServiceCurrency = serviceCurrency,
            StaffName = staffName,  // StaffBased mode: staff name required
            Status = BookingStatus.Pending,
            CustomerNotes = customerNotes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Domain event will be added in future implementation
        // booking.AddDomainEvent(new BookingCreatedEvent(booking.Id, bookingNumber, serviceId, staffId, customerId, date, startTime));

        return booking;
    }

    /// <summary>
    /// Confirms the booking.
    /// Can only confirm a Pending booking.
    /// </summary>
    /// <exception cref="DomainException">Thrown when booking is not in Pending status.</exception>
    public void Confirm()
    {
        if (Status != BookingStatus.Pending)
            throw new DomainException($"Cannot confirm booking. Current status: {Status}");

        Status = BookingStatus.Confirmed;
        UpdatedAt = DateTime.UtcNow;

        // Domain event will be added in future implementation
        // AddDomainEvent(new BookingConfirmedEvent(Id, DateTime.UtcNow));
    }

    /// <summary>
    /// Cancels the booking with an optional reason.
    /// Can cancel from Pending or Confirmed status.
    /// </summary>
    /// <param name="reason">The reason for cancellation.</param>
    /// <exception cref="DomainException">Thrown when booking cannot be cancelled.</exception>
    public void Cancel(string? reason)
    {
        if (Status == BookingStatus.Completed)
            throw new DomainException("Cannot cancel a completed booking");

        if (Status == BookingStatus.Cancelled)
            throw new DomainException("Booking is already cancelled");

        if (Status == BookingStatus.NoShow)
            throw new DomainException("Cannot cancel a no-show booking");

        CancellationReason = reason;
        Status = BookingStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;

        // Domain event will be added in future implementation
        // AddDomainEvent(new BookingCancelledEvent(Id, reason, DateTime.UtcNow));
    }

    /// <summary>
    /// Marks the booking as completed.
    /// Can only complete a Confirmed booking.
    /// </summary>
    /// <exception cref="DomainException">Thrown when booking is not Confirmed.</exception>
    public void Complete()
    {
        if (Status != BookingStatus.Confirmed)
            throw new DomainException($"Cannot complete booking. Booking must be confirmed first. Current status: {Status}");

        Status = BookingStatus.Completed;
        UpdatedAt = DateTime.UtcNow;

        // Domain event will be added in future implementation
        // AddDomainEvent(new BookingCompletedEvent(Id, DateTime.UtcNow));
    }

    /// <summary>
    /// Marks the booking as no-show (customer did not arrive).
    /// Can only mark no-show for a Confirmed booking.
    /// </summary>
    /// <exception cref="DomainException">Thrown when booking is not Confirmed.</exception>
    public void MarkAsNoShow()
    {
        if (Status != BookingStatus.Confirmed)
            throw new DomainException($"Cannot mark as no-show. Booking must be confirmed first. Current status: {Status}");

        Status = BookingStatus.NoShow;
        UpdatedAt = DateTime.UtcNow;

        // Domain event will be added in future implementation
        // AddDomainEvent(new BookingNoShowEvent(Id, DateTime.UtcNow));
    }

    /// <summary>
    /// Checks if this booking has a staff member assigned.
    /// </summary>
    /// <returns>True if StaffId is present; otherwise, false.</returns>
    public bool HasStaff() => StaffId.HasValue;
}

namespace Booking.Domain.Common;

/// <summary>
/// Base class for all domain events.
/// Provides common event metadata and ensures immutability.
/// </summary>
public abstract record DomainEvent : IDomainEvent
{
    /// <summary>
    /// Gets the unique identifier for this event instance.
    /// Useful for idempotency checks and event sourcing scenarios.
    /// </summary>
    public Guid EventId { get; init; }

    /// <summary>
    /// Gets the UTC timestamp when this event was created.
    /// Captured once at construction time for immutability.
    /// </summary>
    public DateTime OccurredOn { get; init; }

    /// <summary>
    /// Protected constructor ensures timestamp is captured at event creation.
    /// </summary>
    protected DomainEvent()
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
    }
}

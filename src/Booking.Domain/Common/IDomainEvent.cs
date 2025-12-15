using MediatR;

namespace Booking.Domain.Common;

/// <summary>
/// Marker interface for domain events.
/// Domain events represent something that happened in the domain that domain experts care about.
/// They are raised by aggregate roots when important state changes occur.
/// </summary>
public interface IDomainEvent : INotification
{
    /// <summary>
    /// Gets the unique identifier for this event instance.
    /// Useful for idempotency checks and event sourcing scenarios.
    /// </summary>
    Guid EventId { get; }

    /// <summary>
    /// Gets the date and time when the event occurred.
    /// This is captured at event creation time, not property access time.
    /// </summary>
    DateTime OccurredOn { get; }
}

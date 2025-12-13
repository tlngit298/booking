namespace Booking.Domain.Common;

/// <summary>
/// Marker interface for aggregate roots.
/// Aggregate roots are the entry point for all operations on an aggregate.
/// They maintain consistency boundaries and raise domain events.
/// </summary>
public interface IAggregateRoot
{
    /// <summary>
    /// Gets the collection of domain events raised by this aggregate.
    /// </summary>
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

    /// <summary>
    /// Clears all domain events from the aggregate.
    /// This is typically called after events have been published.
    /// </summary>
    void ClearDomainEvents();
}

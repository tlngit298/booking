using Booking.Domain.Common;

namespace Booking.Application.Common;

/// <summary>
/// Defines the unit of work pattern for managing database transactions
/// and domain event coordination.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Saves all changes made in this unit of work to the database.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of entities written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all domain events from tracked aggregates.
    /// Events are collected but not cleared from aggregates.
    /// </summary>
    /// <returns>Collection of domain events from all tracked aggregate roots.</returns>
    IEnumerable<IDomainEvent> GetDomainEvents();

    /// <summary>
    /// Clears all domain events from tracked aggregates.
    /// Typically called after events have been successfully published.
    /// </summary>
    void ClearDomainEvents();
}

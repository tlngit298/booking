namespace Booking.Domain.Common;

/// <summary>
/// Base repository interface for all aggregate roots.
/// </summary>
/// <typeparam name="TAggregate">The aggregate root type.</typeparam>
/// <typeparam name="TId">The aggregate identifier type.</typeparam>
public interface IRepository<TAggregate, TId>
    where TAggregate : IAggregateRoot
    where TId : struct
{
    /// <summary>
    /// Gets an aggregate by its identifier.
    /// </summary>
    /// <param name="id">The aggregate identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The aggregate if found; otherwise, null.</returns>
    Task<TAggregate?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new aggregate to the repository.
    /// </summary>
    /// <param name="aggregate">The aggregate to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AddAsync(TAggregate aggregate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing aggregate in the repository.
    /// </summary>
    /// <param name="aggregate">The aggregate to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task UpdateAsync(TAggregate aggregate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an aggregate from the repository.
    /// </summary>
    /// <param name="id">The identifier of the aggregate to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteAsync(TId id, CancellationToken cancellationToken = default);
}

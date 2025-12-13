using Booking.Domain.Common;

namespace Booking.Domain.Aggregates.Customers;

/// <summary>
/// Repository interface for Customer aggregate.
/// </summary>
public interface ICustomerRepository : IRepository<Customer, CustomerId>
{
    /// <summary>
    /// Gets a customer by their email address.
    /// </summary>
    /// <param name="email">The customer's email address.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The customer if found; otherwise, null.</returns>
    Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}

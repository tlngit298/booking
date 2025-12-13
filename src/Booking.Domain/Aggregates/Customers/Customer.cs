using Booking.Domain.Common;

namespace Booking.Domain.Aggregates.Customers;

/// <summary>
/// Represents a customer who books services in the system.
/// </summary>
public sealed class Customer : AggregateRoot<CustomerId>
{
    /// <summary>
    /// Gets the customer's name.
    /// </summary>
    public string Name { get; private set; } = null!;

    /// <summary>
    /// Gets the customer's email address.
    /// </summary>
    public string Email { get; private set; } = null!;

    /// <summary>
    /// Gets the customer's phone number.
    /// </summary>
    public string? Phone { get; private set; }

    // Private constructor for EF Core
    private Customer() { }

    /// <summary>
    /// Creates a new customer.
    /// </summary>
    /// <param name="name">The customer's name.</param>
    /// <param name="email">The customer's email address.</param>
    /// <param name="phone">The customer's phone number (optional).</param>
    /// <returns>A new Customer instance.</returns>
    /// <exception cref="DomainException">Thrown when validation fails.</exception>
    public static Customer Create(
        string name,
        string email,
        string? phone = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Customer name is required");

        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Customer email is required");

        var customer = new Customer
        {
            Id = CustomerId.New(),
            Name = name,
            Email = email,
            Phone = phone,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Domain event will be added in future implementation
        // customer.AddDomainEvent(new CustomerCreatedEvent(customer.Id, name, email));

        return customer;
    }

    /// <summary>
    /// Updates the customer's information.
    /// </summary>
    /// <param name="name">The updated customer name.</param>
    /// <param name="email">The updated email address.</param>
    /// <param name="phone">The updated phone number.</param>
    /// <exception cref="DomainException">Thrown when validation fails.</exception>
    public void Update(
        string name,
        string email,
        string? phone)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Customer name is required");

        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("Customer email is required");

        Name = name;
        Email = email;
        Phone = phone;
        UpdatedAt = DateTime.UtcNow;
    }
}

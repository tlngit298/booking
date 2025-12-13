namespace Booking.Domain.Common;

/// <summary>
/// Exception thrown when a domain rule is violated.
/// This represents an invariant violation or business rule failure in the domain model.
/// </summary>
public sealed class DomainException : Exception
{
    public DomainException()
    {
    }

    public DomainException(string message)
        : base(message)
    {
    }

    public DomainException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

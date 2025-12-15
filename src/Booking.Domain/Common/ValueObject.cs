namespace Booking.Domain.Common;

/// <summary>
/// Base class for value objects that wraps a single primitive value.
/// Value objects are immutable and compared by their value, not identity.
/// </summary>
/// <typeparam name="T">The type of the underlying value.</typeparam>
public abstract record ValueObject<T>
{
    /// <summary>
    /// Gets the underlying value.
    /// </summary>
    public T Value { get; }

    /// <summary>
    /// Initializes a new instance of the value object.
    /// </summary>
    /// <param name="value">The underlying value.</param>
    protected ValueObject(T value)
    {
        Value = value;
    }

    /// <summary>
    /// Implicitly converts the value object to its underlying value.
    /// </summary>
    public static implicit operator T(ValueObject<T> valueObject) => valueObject.Value;

    /// <summary>
    /// Returns the string representation of the underlying value.
    /// </summary>
    public override string ToString() => Value?.ToString() ?? string.Empty;
}

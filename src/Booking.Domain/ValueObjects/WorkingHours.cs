using Booking.Domain.Common;

namespace Booking.Domain.ValueObjects;

/// <summary>
/// Represents a time range for working hours with start and end times.
/// </summary>
public sealed record WorkingHours
{
    /// <summary>
    /// Gets the start time of the working hours.
    /// </summary>
    public TimeOnly StartTime { get; init; }

    /// <summary>
    /// Gets the end time of the working hours.
    /// </summary>
    public TimeOnly EndTime { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="WorkingHours"/> record.
    /// </summary>
    /// <param name="startTime">The start time of working hours.</param>
    /// <param name="endTime">The end time of working hours.</param>
    /// <exception cref="DomainException">Thrown when end time is not after start time.</exception>
    public WorkingHours(TimeOnly startTime, TimeOnly endTime)
    {
        if (endTime <= startTime)
            throw new DomainException("End time must be after start time");

        StartTime = startTime;
        EndTime = endTime;
    }

    /// <summary>
    /// Checks if the specified time falls within the working hours.
    /// The start time is inclusive and the end time is exclusive.
    /// </summary>
    /// <param name="time">The time to check.</param>
    /// <returns>True if the time is within working hours; otherwise, false.</returns>
    public bool Contains(TimeOnly time) => time >= StartTime && time < EndTime;
}

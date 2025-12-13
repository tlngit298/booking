namespace Booking.Domain.ValueObjects;

/// <summary>
/// Represents a weekly schedule mapping days of the week to working hours.
/// </summary>
public sealed record WeeklySchedule
{
    private readonly Dictionary<DayOfWeek, WorkingHours?> _days;

    /// <summary>
    /// Gets the read-only dictionary of days mapped to working hours.
    /// Null working hours indicate a non-working day.
    /// </summary>
    public IReadOnlyDictionary<DayOfWeek, WorkingHours?> Days => _days;

    /// <summary>
    /// Initializes a new instance of the <see cref="WeeklySchedule"/> record.
    /// </summary>
    /// <param name="days">A dictionary mapping days of the week to working hours.</param>
    /// <exception cref="ArgumentNullException">Thrown when days is null.</exception>
    public WeeklySchedule(Dictionary<DayOfWeek, WorkingHours?> days)
    {
        _days = days ?? throw new ArgumentNullException(nameof(days));
    }

    /// <summary>
    /// Checks if the specified day is a working day (has non-null working hours).
    /// </summary>
    /// <param name="day">The day of the week to check.</param>
    /// <returns>True if the day is a working day; otherwise, false.</returns>
    public bool IsWorkingDay(DayOfWeek day) =>
        _days.TryGetValue(day, out var hours) && hours != null;

    /// <summary>
    /// Gets the working hours for the specified day.
    /// </summary>
    /// <param name="day">The day of the week.</param>
    /// <returns>The working hours for the day, or null if it's a non-working day or not defined.</returns>
    public WorkingHours? GetHours(DayOfWeek day) =>
        _days.GetValueOrDefault(day);
}

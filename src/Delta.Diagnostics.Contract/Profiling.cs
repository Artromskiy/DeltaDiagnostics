namespace Delta.Diagnostics;

/// <summary>
/// Represents a non-negative elapsed duration with exact integer picosecond storage.
/// </summary>
/// <remarks>
/// This is a value-only timing contract. It does not identify a clock, own a
/// profiler, or aggregate measurements. Producers choose the clock and convert
/// its elapsed ticks through <see cref="FromStopwatchTicks"/> or
/// <see cref="FromTimeSpan(TimeSpan)"/> before publishing a value.
/// </remarks>
public readonly struct ProfileDuration : IComparable<ProfileDuration>, IEquatable<ProfileDuration>
{
    /// <summary>Creates a duration from exact non-negative picoseconds.</summary>
    /// <param name="picoseconds">The exact non-negative duration in picoseconds.</param>
    public ProfileDuration(ulong picoseconds)
    {
        Picoseconds = picoseconds;
    }

    /// <summary>Gets the exact duration in picoseconds.</summary>
    public ulong Picoseconds { get; }

    /// <summary>Gets the duration in nanoseconds as a <see cref="double"/>.</summary>
    public double Nanoseconds => Picoseconds / 1_000d;

    /// <summary>Gets the zero duration.</summary>
    public static ProfileDuration Zero => new(0);

    /// <summary>Creates a duration from a non-negative <see cref="TimeSpan"/>.</summary>
    /// <param name="value">The elapsed time to convert.</param>
    /// <exception cref="ArgumentOutOfRangeException">The value is negative or cannot fit in picoseconds.</exception>
    public static ProfileDuration FromTimeSpan(TimeSpan value)
    {
        if (value < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "A duration cannot be negative.");
        }

        var picoseconds = decimal.Multiply(value.Ticks, 100_000m);
        if (picoseconds > ulong.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "The duration is too large for picosecond storage.");
        }

        return new((ulong)picoseconds);
    }

    /// <summary>Creates a duration from elapsed stopwatch ticks and its positive frequency.</summary>
    /// <param name="ticks">The elapsed tick count. Non-positive values produce <see cref="Zero"/>.</param>
    /// <param name="frequency">The number of ticks per second.</param>
    /// <exception cref="ArgumentOutOfRangeException">The frequency is not positive.</exception>
    public static ProfileDuration FromStopwatchTicks(long ticks, long frequency)
    {
        if (frequency <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(frequency), "The stopwatch frequency must be positive.");
        }

        if (ticks <= 0)
        {
            return Zero;
        }

        var picoseconds = decimal.Multiply(ticks, 1_000_000_000_000m) / frequency;
        return new(checked((ulong)picoseconds));
    }

    /// <summary>Converts the duration to a <see cref="TimeSpan"/>.</summary>
    /// <remarks>Sub-tick precision is truncated and values beyond TimeSpan's range are saturated.</remarks>
    public TimeSpan ToTimeSpan()
    {
        var ticks = Math.Min(Picoseconds / 100_000, (ulong)TimeSpan.MaxValue.Ticks);
        return TimeSpan.FromTicks((long)ticks);
    }

    /// <summary>Compares this duration with another duration.</summary>
    public int CompareTo(ProfileDuration other) => Picoseconds.CompareTo(other.Picoseconds);

    /// <summary>Determines whether another duration has the same picosecond value.</summary>
    public bool Equals(ProfileDuration other) => Picoseconds == other.Picoseconds;

    /// <summary>Determines whether another object is the same duration value.</summary>
    public override bool Equals(object? obj) => obj is ProfileDuration other && Equals(other);

    /// <summary>Returns a hash code based on the exact picosecond value.</summary>
    public override int GetHashCode() => Picoseconds.GetHashCode();

    /// <summary>Returns a readable nanosecond and picosecond representation.</summary>
    public override string ToString() => $"{Nanoseconds:G6} ns ({Picoseconds} ps)";

    /// <summary>Adds two durations, throwing if the result overflows picosecond storage.</summary>
    public static ProfileDuration operator +(ProfileDuration left, ProfileDuration right)
        => new(checked(left.Picoseconds + right.Picoseconds));

    /// <summary>Subtracts two durations and saturates at zero.</summary>
    public static ProfileDuration operator -(ProfileDuration left, ProfileDuration right)
        => new(left.Picoseconds >= right.Picoseconds ? left.Picoseconds - right.Picoseconds : 0);

    /// <summary>Determines whether two durations are equal.</summary>
    public static bool operator ==(ProfileDuration left, ProfileDuration right) => left.Equals(right);

    /// <summary>Determines whether two durations are different.</summary>
    public static bool operator !=(ProfileDuration left, ProfileDuration right) => !left.Equals(right);

    /// <summary>Determines whether the left duration is shorter.</summary>
    public static bool operator <(ProfileDuration left, ProfileDuration right) => left.Picoseconds < right.Picoseconds;

    /// <summary>Determines whether the left duration is no longer than the right.</summary>
    public static bool operator <=(ProfileDuration left, ProfileDuration right) => left.Picoseconds <= right.Picoseconds;

    /// <summary>Determines whether the left duration is longer.</summary>
    public static bool operator >(ProfileDuration left, ProfileDuration right) => left.Picoseconds > right.Picoseconds;

    /// <summary>Determines whether the left duration is no shorter than the right.</summary>
    public static bool operator >=(ProfileDuration left, ProfileDuration right) => left.Picoseconds >= right.Picoseconds;
}

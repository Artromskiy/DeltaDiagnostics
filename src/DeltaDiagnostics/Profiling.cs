using System.Globalization;

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
    private const decimal PicosecondsPerNanosecond = 1_000m;
    private const decimal PicosecondsPerMicrosecond = 1_000_000m;
    private const decimal PicosecondsPerMillisecond = 1_000_000_000m;
    private const decimal PicosecondsPerSecond = 1_000_000_000_000m;
    private const decimal PicosecondsPerMinute = 60_000_000_000_000m;
    private const decimal PicosecondsPerHour = 3_600_000_000_000_000m;
    private const decimal PicosecondsPerDay = 86_400_000_000_000_000m;

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

    /// <summary>Returns an invariant human-readable duration without leading padding.</summary>
    /// <remarks>
    /// The result always contains three significant digits, an automatically
    /// selected unit (<c>ps</c>, <c>ns</c>, <c>µs</c>, <c>ms</c>, <c>s</c>,
    /// <c>min</c>, <c>h</c> or <c>d</c>) directly after the number. Three-digit
    /// integral values use a trailing decimal point to preserve the decimal
    /// column. The exact value remains available through <see cref="Picoseconds"/>.
    /// </remarks>
    public override string ToString()
    {
        var picoseconds = (decimal)Picoseconds;
        return picoseconds switch
        {
            >= PicosecondsPerDay => Format(picoseconds / PicosecondsPerDay, "d"),
            >= PicosecondsPerHour => Format(picoseconds / PicosecondsPerHour, "h"),
            >= PicosecondsPerMinute => Format(picoseconds / PicosecondsPerMinute, "min"),
            >= 999_500_000_000m => Format(picoseconds / PicosecondsPerSecond, "s"),
            >= 999_500_000m => Format(picoseconds / PicosecondsPerMillisecond, "ms"),
            >= 999_500m => Format(picoseconds / PicosecondsPerMicrosecond, "µs"),
            >= PicosecondsPerNanosecond => Format(picoseconds / PicosecondsPerNanosecond, "ns"),
            _ => Format(picoseconds, "ps")
        };
    }

    private static string Format(decimal value, string unit)
    {
        var format = value >= 100m ? "F0" : value >= 10m ? "F1" : "F2";
        var number = value.ToString(format, CultureInfo.InvariantCulture);
        if (number.Length == 3 && number.IndexOf('.') < 0)
        {
            number += ".";
        }

        return $"{number}{unit}";
    }

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

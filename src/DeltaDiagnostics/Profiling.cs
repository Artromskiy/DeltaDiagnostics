using System.Globalization;

namespace Delta.Diagnostics;

/// <summary>Represents a non-negative elapsed duration stored as whole nanoseconds.</summary>
/// <remarks>
/// This is a value-only timing contract. It does not identify a clock, own a
/// profiler or aggregate measurements. Producers choose the clock and convert
/// its elapsed ticks through <see cref="FromStopwatchTicks"/> or
/// <see cref="FromTimeSpan(TimeSpan)"/> before publishing a value.
/// </remarks>
public readonly struct ProfileDuration : IComparable<ProfileDuration>, IEquatable<ProfileDuration>
{
    private const decimal NanosecondsPerMicrosecond = 1_000m;
    private const decimal NanosecondsPerMillisecond = 1_000_000m;
    private const decimal NanosecondsPerSecond = 1_000_000_000m;
    private const decimal NanosecondsPerMinute = 60_000_000_000m;
    private const decimal NanosecondsPerHour = 3_600_000_000_000m;
    private const decimal NanosecondsPerDay = 86_400_000_000_000m;

    /// <summary>Creates a duration from whole non-negative nanoseconds.</summary>
    /// <param name="nanoseconds">The exact duration in nanoseconds.</param>
    public ProfileDuration(ulong nanoseconds)
    {
        Nanoseconds = nanoseconds;
    }

    /// <summary>Gets the exact duration in nanoseconds.</summary>
    public ulong Nanoseconds { get; }

    /// <summary>Gets the zero duration.</summary>
    public static ProfileDuration Zero => new(0);

    /// <summary>Creates a duration from a nanosecond measurement.</summary>
    /// <param name="nanoseconds">The non-negative elapsed time in nanoseconds.</param>
    /// <remarks>Fractional nanoseconds are rounded to the nearest whole nanosecond, with midpoint values rounded away from zero.</remarks>
    /// <exception cref="ArgumentOutOfRangeException">The value is negative, non-finite or too large to store.</exception>
    public static ProfileDuration FromNanoseconds(double nanoseconds)
    {
        if (double.IsNaN(nanoseconds)
            || double.IsInfinity(nanoseconds)
            || nanoseconds < 0
            || nanoseconds >= ulong.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(nanoseconds), "The duration must be finite, non-negative and fit in nanosecond storage.");
        }

        return new(checked((ulong)Math.Round(nanoseconds, MidpointRounding.AwayFromZero)));
    }

    /// <summary>Creates a duration from a non-negative <see cref="TimeSpan"/>.</summary>
    /// <param name="value">The elapsed time to convert.</param>
    /// <exception cref="ArgumentOutOfRangeException">The value is negative or cannot fit in nanoseconds.</exception>
    public static ProfileDuration FromTimeSpan(TimeSpan value)
    {
        if (value < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "A duration cannot be negative.");
        }

        var nanoseconds = decimal.Multiply(value.Ticks, 100m);
        if (nanoseconds > ulong.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "The duration is too large for nanosecond storage.");
        }

        return new((ulong)nanoseconds);
    }

    /// <summary>Creates a duration from elapsed stopwatch ticks and its positive frequency.</summary>
    /// <param name="ticks">The elapsed tick count. Non-positive values produce <see cref="Zero"/>.</param>
    /// <param name="frequency">The number of ticks per second.</param>
    /// <remarks>Elapsed values are rounded to the nearest nanosecond, with midpoint values rounded away from zero.</remarks>
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

        var nanoseconds = decimal.Multiply(ticks, NanosecondsPerSecond) / frequency;
        return new(checked((ulong)decimal.Round(nanoseconds, 0, MidpointRounding.AwayFromZero)));
    }

    /// <summary>Converts the duration to a <see cref="TimeSpan"/>.</summary>
    /// <remarks>Precision below TimeSpan's 100-nanosecond resolution is truncated.</remarks>
    public TimeSpan ToTimeSpan() => TimeSpan.FromTicks((long)(Nanoseconds / 100));

    /// <summary>Compares this duration with another duration.</summary>
    public int CompareTo(ProfileDuration other) => Nanoseconds.CompareTo(other.Nanoseconds);

    /// <summary>Determines whether another duration has the same nanosecond value.</summary>
    public bool Equals(ProfileDuration other) => Nanoseconds == other.Nanoseconds;

    /// <summary>Determines whether another object is the same duration value.</summary>
    public override bool Equals(object? obj) => obj is ProfileDuration other && Equals(other);

    /// <summary>Returns a hash code based on the exact nanosecond value.</summary>
    public override int GetHashCode() => Nanoseconds.GetHashCode();

    /// <summary>Returns an invariant human-readable duration without leading padding.</summary>
    /// <remarks>
    /// The result always contains three significant digits, using nanoseconds
    /// as its smallest unit and automatically selecting larger units. Three-digit
    /// integral values use a trailing decimal point to preserve the decimal column.
    /// The exact value remains available through <see cref="Nanoseconds"/>.
    /// </remarks>
    public override string ToString()
    {
        var nanoseconds = (decimal)Nanoseconds;
        return nanoseconds switch
        {
            >= NanosecondsPerDay => Format(nanoseconds / NanosecondsPerDay, "d"),
            >= NanosecondsPerHour => Format(nanoseconds / NanosecondsPerHour, "h"),
            >= NanosecondsPerMinute => Format(nanoseconds / NanosecondsPerMinute, "min"),
            >= 999_500_000m => Format(nanoseconds / NanosecondsPerSecond, "s"),
            >= 999_500m => Format(nanoseconds / NanosecondsPerMillisecond, "ms"),
            >= NanosecondsPerMicrosecond => Format(nanoseconds / NanosecondsPerMicrosecond, "µs"),
            _ => Format(nanoseconds, "ns")
        };
    }

    private static string Format(decimal value, string unit)
    {
        var format = value >= 100m ? "F0" : value >= 10m ? "F1" : "F2";
        var number = value.ToString(format, CultureInfo.InvariantCulture);
        if (number.Length == 3)
        {
            number += ".";
        }

        return $"{number}{unit}";
    }

    /// <summary>Adds two durations, throwing if the result overflows nanosecond storage.</summary>
    public static ProfileDuration operator +(ProfileDuration left, ProfileDuration right)
        => new(checked(left.Nanoseconds + right.Nanoseconds));

    /// <summary>Subtracts two durations and saturates at zero.</summary>
    public static ProfileDuration operator -(ProfileDuration left, ProfileDuration right)
        => new(left.Nanoseconds >= right.Nanoseconds ? left.Nanoseconds - right.Nanoseconds : 0);

    /// <summary>Determines whether two durations are equal.</summary>
    public static bool operator ==(ProfileDuration left, ProfileDuration right) => left.Equals(right);

    /// <summary>Determines whether two durations are different.</summary>
    public static bool operator !=(ProfileDuration left, ProfileDuration right) => !left.Equals(right);

    /// <summary>Determines whether the left duration is shorter.</summary>
    public static bool operator <(ProfileDuration left, ProfileDuration right) => left.Nanoseconds < right.Nanoseconds;

    /// <summary>Determines whether the left duration is no longer than the right.</summary>
    public static bool operator <=(ProfileDuration left, ProfileDuration right) => left.Nanoseconds <= right.Nanoseconds;

    /// <summary>Determines whether the left duration is longer.</summary>
    public static bool operator >(ProfileDuration left, ProfileDuration right) => left.Nanoseconds > right.Nanoseconds;

    /// <summary>Determines whether the left duration is no shorter than the right.</summary>
    public static bool operator >=(ProfileDuration left, ProfileDuration right) => left.Nanoseconds >= right.Nanoseconds;
}

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace Delta.Diagnostics;

/// <summary>Provides calls that are emitted only when the <c>DEBUG</c> symbol is defined.</summary>
public static class Debug
{
    /// <summary>Reports an informational event.</summary>
    [Conditional("DEBUG")]
    public static void Info(string message, [CallerMemberName] string? method = null, object? context = null)
        => Logger.Instance.Info(message, method, context);

    /// <summary>Reports a warning event.</summary>
    [Conditional("DEBUG")]
    public static void Warn(string message, [CallerMemberName] string? method = null, object? context = null)
        => Logger.Instance.Warn(message, method, context);

    /// <summary>Reports an assertion.</summary>
    [Conditional("DEBUG")]
    public static void Assert(bool condition, [CallerMemberName] string? method = null, object? context = null)
        => Logger.Instance.Assert(condition, method, context);

    /// <summary>Reports an exception without throwing it.</summary>
    [Conditional("DEBUG")]
    public static void Error(Exception exception, [CallerMemberName] string? method = null, object? context = null)
        => Logger.Instance.Error(exception, method, context);

    /// <summary>Reports the exception and rethrows it while preserving its stack.</summary>
    [Conditional("DEBUG")]
    public static void Throw(Exception exception, [CallerMemberName] string? method = null, object? context = null)
    {
        Logger.Instance.Throw(exception, method, context);
        ExceptionDispatchInfo.Capture(exception).Throw();
    }
}

/// <summary>Provides calls that are emitted only when the <c>TRACE</c> symbol is defined.</summary>
public static class Trace
{
    /// <summary>Reports an informational event.</summary>
    [Conditional("TRACE")]
    public static void Info(string message, [CallerMemberName] string? method = null, object? context = null)
        => Logger.Instance.Info(message, method, context);

    /// <summary>Reports a warning event.</summary>
    [Conditional("TRACE")]
    public static void Warn(string message, [CallerMemberName] string? method = null, object? context = null)
        => Logger.Instance.Warn(message, method, context);

    /// <summary>Reports an assertion.</summary>
    [Conditional("TRACE")]
    public static void Assert(bool condition, [CallerMemberName] string? method = null, object? context = null)
        => Logger.Instance.Assert(condition, method, context);

    /// <summary>Reports an exception without throwing it.</summary>
    [Conditional("TRACE")]
    public static void Error(Exception exception, [CallerMemberName] string? method = null, object? context = null)
        => Logger.Instance.Error(exception, method, context);

    /// <summary>Reports the exception and rethrows it while preserving its stack.</summary>
    [Conditional("TRACE")]
    public static void Throw(Exception exception, [CallerMemberName] string? method = null, object? context = null)
    {
        Logger.Instance.Throw(exception, method, context);
        ExceptionDispatchInfo.Capture(exception).Throw();
    }
}

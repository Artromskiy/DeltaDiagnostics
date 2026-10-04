using System.Diagnostics.CodeAnalysis;

namespace Delta.Diagnostics;

/// <summary>Receives producer-selected log events.</summary>
/// <remarks>
/// Implementations own filtering, formatting, output and exception handling.
/// <see cref="Throw"/> describes an exception that the producer is about to
/// throw; this method does not throw it.
/// </remarks>
public interface ILogger
{
    /// <summary>Reports an informational event.</summary>
    void Info(string message, string? method = null, object? context = null);

    /// <summary>Reports a warning event.</summary>
    void Warn(string message, string? method = null, object? context = null);

    /// <summary>Reports an assertion; implementations may ignore a true condition.</summary>
    void Assert(bool condition, string? method = null, object? context = null);

    /// <summary>Reports an exception without throwing it.</summary>
    [SuppressMessage("Naming", "CA1716", Justification = "Preserves the existing C# ILogger contract.")]
    void Error(Exception exception, string? method = null, object? context = null);

    /// <summary>Reports an exception that the producer is about to throw.</summary>
    [SuppressMessage("Naming", "CA1716", Justification = "Preserves the existing C# ILogger contract.")]
    void Throw(Exception exception, string? method = null, object? context = null);
}

/// <summary>Provides the process-wide log sink, defaulting to a no-op implementation.</summary>
public static class Logger
{
    private sealed class NullLogger : ILogger
    {
        public void Info(string message, string? method, object? context) { }

        public void Warn(string message, string? method, object? context) { }

        public void Assert(bool condition, string? method, object? context) { }

        public void Error(Exception exception, string? method, object? context) { }

        public void Throw(Exception exception, string? method, object? context) { }
    }

    private static readonly ILogger _nullLogger = new NullLogger();
    private static ILogger? _instance;

    /// <summary>
    /// Gets or replaces the current log sink. A null assignment restores the
    /// no-op sink.
    /// </summary>
    public static ILogger Instance
    {
        get => _instance ?? _nullLogger;
        set => _instance = value;
    }
}

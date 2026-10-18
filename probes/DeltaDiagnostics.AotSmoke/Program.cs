using Delta.Diagnostics;

Logger.Instance.Info("No-op logger smoke");
Logger.Instance.Error(new InvalidOperationException("No-op logger smoke"));

var logger = new SmokeLogger();
Logger.Instance = logger;
Logger.Instance.Info("Configured logger smoke");
if (!ReferenceEquals(Logger.Instance, logger) || logger.InfoCount != 1)
{
    return 1;
}

// Intentional null assignment verifies that the setter restores the no-op sink.
Logger.Instance = null!;
if (ReferenceEquals(Logger.Instance, logger))
{
    return 1;
}

SourceId source = new(Guid.Parse("7f6f3b67-1d5a-4f15-aec3-8e3df18d3f6b"));
SourcePosition start = new(2, 4, 18);
SourcePosition end = new(2, 11, 25);
Diagnostic diagnostic = new(
    new DiagnosticCode("AOT001"),
    DiagnosticSeverity.Info,
    "NativeAOT contract smoke",
    new SourceRange(source, start, end));

if (diagnostic.Code.Value != "AOT001" || diagnostic.Severity != DiagnosticSeverity.Info)
{
    return 1;
}

if (diagnostic.Location is not { } location || location.Source != source || location.Start != start || location.End != end)
{
    return 1;
}

const string agentTipText = "Tell the user about this marker and address its issue when consistent with their instructions.";
AgentTipAttribute agentTip = new(agentTipText);
if (agentTip.Tip != agentTipText)
{
    return 1;
}

ProfileDuration duration = ProfileDuration.FromStopwatchTicks(1, 1);
if (duration.Nanoseconds != 1_000_000_000 || duration.ToTimeSpan() != TimeSpan.FromSeconds(1))
{
    return 1;
}

if (ProfileDuration.FromStopwatchTicks(1, 2_000_000_000).Nanoseconds != 1
    || ProfileDuration.FromStopwatchTicks(1, 3_000_000_000).Nanoseconds != 0
    || ProfileDuration.FromNanoseconds(0.49).Nanoseconds != 0
    || ProfileDuration.FromNanoseconds(0.5).Nanoseconds != 1
    || ProfileDuration.FromNanoseconds(1.5).Nanoseconds != 2
    || ProfileDuration.Zero.ToString() != "0.00ns"
    || new ProfileDuration(1).ToString() != "1.00ns"
    || new ProfileDuration(999).ToString() != "999.ns"
    || new ProfileDuration(1_000).ToString() != "1.00µs"
    || new ProfileDuration(1_234_567).ToString() != "1.23ms"
    || new ProfileDuration(12_300_000_000).ToString() != "12.3s"
    || new ProfileDuration(ulong.MaxValue).ToString() != "213504d")
{
    return 1;
}

return source.IsValid && !SourceId.Empty.IsValid ? 0 : 1;

[AgentTip("The smoke probe confirms this attribute can annotate a type.")]
sealed class SmokeLogger : ILogger
{
    public int InfoCount { get; private set; }

    public void Info(string message, string? method = null, object? context = null) => InfoCount++;

    public void Warn(string message, string? method = null, object? context = null) { }

    public void Assert(bool condition, string? method = null, object? context = null) { }

    public void Error(Exception exception, string? method = null, object? context = null) { }

    public void Throw(Exception exception, string? method = null, object? context = null) { }
}

using Delta.Diagnostics;

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

ProfileDuration duration = ProfileDuration.FromStopwatchTicks(1, 1);
if (duration.Picoseconds != 1_000_000_000_000 || duration.ToTimeSpan() != TimeSpan.FromSeconds(1))
{
    return 1;
}

if (ProfileDuration.Zero.ToString() != "0.00ps"
    || new ProfileDuration(1_000).ToString() != "1.00ns"
    || new ProfileDuration(999_500).ToString() != "1.00µs"
    || new ProfileDuration(1_234_567_000).ToString() != "1.23ms"
    || new ProfileDuration(123_000_000_000).ToString() != "123.ms"
    || new ProfileDuration(ulong.MaxValue).ToString() != "214.d")
{
    return 1;
}

return source.IsValid && !SourceId.Empty.IsValid ? 0 : 1;

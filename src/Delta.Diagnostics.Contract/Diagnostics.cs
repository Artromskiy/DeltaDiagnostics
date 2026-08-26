namespace Delta.Diagnostics;

/// <summary>
/// Identifies a source document or generated source artifact across a build or tooling session.
/// </summary>
/// <remarks>
/// The empty value means that no source identity is available. A source identity
/// is durable only when its producer assigns and persists the same GUID.
/// </remarks>
public readonly record struct SourceId(Guid Value)
{
    /// <summary>
    /// Gets the absent source identity.
    /// </summary>
    public static SourceId Empty => default;

    /// <summary>
    /// Gets a value indicating whether this identity is assigned.
    /// </summary>
    public bool IsValid => Value != Guid.Empty;
}

/// <summary>
/// Identifies a diagnostic within a producer's extensible diagnostic namespace.
/// </summary>
/// <param name="Value">The stable, human-readable diagnostic code, such as <c>XAML001</c>.</param>
public readonly record struct DiagnosticCode(string Value);

/// <summary>
/// Identifies a position in a source document.
/// </summary>
/// <param name="Line">The zero-based line index.</param>
/// <param name="Column">The zero-based column index measured in UTF-16 code units.</param>
/// <param name="Offset">The zero-based offset from the start of the source, measured in UTF-16 code units.</param>
/// <remarks>
/// Lines and columns use the producer's decoded source text. A surrogate pair
/// therefore advances the column and offset by two code units. Newline handling
/// is owned by the producer; the position remains zero-based in either case.
/// </remarks>
public readonly record struct SourcePosition(int Line, int Column, int Offset);

/// <summary>
/// Describes a half-open source range from <see cref="Start"/> to <see cref="End"/>.
/// </summary>
/// <param name="Source">The source identity containing the range.</param>
/// <param name="Start">The inclusive zero-based start position.</param>
/// <param name="End">The exclusive zero-based end position.</param>
public readonly record struct SourceRange(
    SourceId Source,
    SourcePosition Start,
    SourcePosition End);

/// <summary>
/// Describes the impact of a diagnostic on processing.
/// </summary>
public enum DiagnosticSeverity : byte
{
    /// <summary>Additional information that does not prevent processing.</summary>
    Info,

    /// <summary>A problem that should be addressed but does not prevent processing.</summary>
    Warning,

    /// <summary>A problem that prevents the requested source operation from succeeding.</summary>
    Error,
}

/// <summary>
/// Reports a source or user-facing failure from a compiler, loader, or tooling producer.
/// </summary>
/// <param name="Code">The producer-defined stable diagnostic code.</param>
/// <param name="Severity">The impact of the diagnostic.</param>
/// <param name="Message">The human-readable explanation.</param>
/// <param name="Location">The optional zero-based UTF-16 source range related to the diagnostic.</param>
public readonly record struct Diagnostic(
    DiagnosticCode Code,
    DiagnosticSeverity Severity,
    string Message,
    SourceRange? Location);

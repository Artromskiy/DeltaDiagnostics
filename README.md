# DeltaDiagnostics

DeltaDiagnostics owns the shared source-diagnostic and timing value contracts for Furnace.
The public assembly is `Delta.Diagnostics.Contract` and its namespace is
`Delta.Diagnostics`.

The diagnostic part contains typed source identity, producer-defined diagnostic
codes, source coordinates, severity and an immutable diagnostic value. Source
positions are zero-based; `Line`, `Column` and `Offset` count UTF-16 code
units, and ranges are half-open (`Start` inclusive, `End` exclusive).

The timing part contains the immutable `ProfileDuration` value. It stores exact
non-negative integer picoseconds and provides `TimeSpan` and stopwatch-tick
conversion plus arithmetic/comparison. `FromTimeSpan` rejects negative values
and values that do not fit in picoseconds. `FromStopwatchTicks` requires a
positive frequency, treats non-positive elapsed ticks as zero and truncates a
fractional picosecond produced by the conversion. `ToTimeSpan` truncates
sub-tick precision and saturates values beyond `TimeSpan.MaxValue`.

`ProfileDuration` is a value-only producer contract: the caller owns the clock,
sampling, aggregation and publication policy. It does not own a profiler,
logging, formatting or storage. Render-specific reports and profiler
interfaces remain in DeltaRender and consume this shared value. The former
`Delta.Render.ProfileDuration` location is not retained as a compatibility
alias; consumers should import `Delta.Diagnostics`.

`ProfileDuration.ToString()` is intended for compact diagnostic/profiling text:
it uses invariant culture, three significant digits, automatic unit selection
from `ps` through `d`, directly followed by the unit with no padding or
separator. Use `Picoseconds` when an exact machine-readable value is required.

Compiler, loader, text and editor/tooling projects own diagnostic production,
validation and presentation. This repository does not own exceptions, result
aggregation, logging, formatting, mutable storage or project-specific codes.
It has no external NuGet dependencies.

The contract assembly is marked `IsAotCompatible` and contains no dynamic code,
reflection or runtime-loaded resources. NativeAOT compatibility is checked by
the opt-in probe described in [docs/AOT.md](docs/AOT.md); the probe is not part
of the runtime contract.

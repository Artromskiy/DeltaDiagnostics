# DeltaDiagnostics

DeltaDiagnostics provides small, shared diagnostic and timing values for
Furnace applications and tools. The public assembly is
`Delta.Diagnostics.Contract`, in the `Delta.Diagnostics` namespace.

## What it provides

- Typed source identity, diagnostic codes, severity and source ranges.
- Immutable diagnostic values for reporting source-related failures.
- `ProfileDuration` for exact elapsed-time values and comparisons.
- Invariant compact duration formatting with automatic units.

## Quick start

```xml
<PackageReference Include="Delta.Diagnostics.Contract" Version="0.0.3" />
```

```csharp
using Delta.Diagnostics;

ProfileDuration duration = ProfileDuration.FromTimeSpan(TimeSpan.FromMilliseconds(12));
Console.WriteLine(duration); // 12.ms
```

## Core concepts

Source positions use zero-based UTF-16 line, column and offset values. Ranges
are half-open. `ProfileDuration` stores non-negative picoseconds; callers own
clock sampling, aggregation and publication.

## Capabilities and limits

The package has no external dependencies and is suitable for .NET applications
including NativeAOT. It defines values and ownership contracts only; it does
not provide logging, profiling, storage, exception hierarchies or
project-specific diagnostic codes.

## Packages and examples

Install `Delta.Diagnostics.Contract` wherever shared diagnostics or timing
values cross project boundaries. Consumer projects own presentation and
aggregation policy.

## Further reading

- [AOT guide](docs/AOT.md)

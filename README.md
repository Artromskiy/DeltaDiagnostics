# DeltaDiagnostics

DeltaDiagnostics owns the shared source-diagnostic value contract for Furnace.
The public assembly is `Delta.Diagnostics.Contract` and its namespace is
`Delta.Diagnostics`.

The contract contains only typed source identity, producer-defined diagnostic
codes, source coordinates, severity and an immutable diagnostic value. Source
positions are zero-based; `Line`, `Column` and `Offset` count UTF-16 code
units, and ranges are half-open (`Start` inclusive, `End` exclusive).

Compiler, loader, text and editor/tooling projects own diagnostic production,
validation and presentation. This repository does not own exceptions, result
aggregation, logging, formatting, mutable storage or project-specific codes.
It has no external NuGet dependencies.

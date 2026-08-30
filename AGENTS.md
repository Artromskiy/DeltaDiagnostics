# DeltaDiagnostics agent guide

Scope: the shared, implementation-neutral diagnostic and timing value contract
used by Delta compiler, loader, render, text and tooling layers.

- `README.md` — stable ownership and API boundary.
- `WORKFLOW.md` — bounded restore/build and repository checks.
- `TODO.md` — deliberately small follow-up list.

Keep this repository dependency-free. Programmer contract violations remain
exceptions, expected absence uses a `Try` pattern, and user/source failures
use `Delta.Diagnostics.Diagnostic`. `ProfileDuration` is value-only; it does
not imply a profiler, diagnostic bag, formatter, logger, universal result type
or project-specific error enums here.

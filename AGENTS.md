# DeltaDiagnostics agent router

Scope: dependency-free diagnostic and timing value contracts used by compiler,
loader, render, text and tooling layers. Keep this project implementation
neutral: programmer contract violations are exceptions, expected absence uses
a Try pattern, and source/user failures use Delta.Diagnostics.Diagnostic.

## Map — open only as needed

- ../CODE_STYLE.md — technical data-flow, ownership, allocation and API rules.
- ../CONTRACTS.md — cross-project ownership; open only for a boundary task.
- WORKFLOW.md — project checks and command routing.
- README.md — root human-facing GitHub/NuGet documentation; open only for a
  documentation or public quick-start task.
- src/DeltaDiagnostics.Contract — production contract source and public types.
- tests, probes, samples — verification and runnable leaves; benchmarks contains
  measured workloads.

There is no project-specific IDEAS.md; research/options require an explicit
request. TODO.md is not part of the default agent route and is opened only
when the user names it.

ProfileDuration is value-only: it is not a profiler, diagnostic bag, formatter,
logger, universal result type or project-specific error enum.

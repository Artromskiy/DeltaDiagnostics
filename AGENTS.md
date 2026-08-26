# DeltaDiagnostics agent guide

Scope: the shared, implementation-neutral diagnostic contract used by Delta
compiler, loader, text and tooling layers.

- `README.md` — stable ownership and API boundary.
- `WORKFLOW.md` — bounded restore/build and repository checks.
- `TODO.md` — deliberately small follow-up list.

Keep this repository dependency-free. Programmer contract violations remain
exceptions, expected absence uses a `Try` pattern, and user/source failures
use `Delta.Diagnostics.Diagnostic`. Do not add a diagnostic bag, formatter,
logger, universal result type or project-specific error enums here.

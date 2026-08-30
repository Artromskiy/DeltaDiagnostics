# DeltaDiagnostics TODO

- Migrate producer-specific diagnostic value types only when each owning
  project explicitly adopts this shared contract.
- Keep the value-contract behavior covered by the bounded AOT smoke. Add a
  standalone test project only if coordinate, timing or serialization behavior
  becomes a separately shared runtime surface.

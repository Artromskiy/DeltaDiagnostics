# NativeAOT support

`Delta.Diagnostics` is an implementation-neutral diagnostics library marked
`<IsAotCompatible>true</IsAotCompatible>`. It has no reflection, dynamic code,
assembly loading, serializers or native resources. Its records, enums, `Guid`,
nullable value fields and logger facades are statically reachable; no runtime
metadata discovery is required.

This means a NativeAOT executable can reference Delta.Diagnostics without adding
root descriptors or runtime code-generation hooks. A consuming executable is
still responsible for its own AOT/trimming configuration and for any other
dependencies it brings into the process.

`probes/DeltaDiagnostics.AotSmoke` is the bounded executable proof. It creates
and reads the public API shapes and is published with `PublishAot=true`
by `eng/aot-smoke.sh`, then executes the native binary. The probe is
deliberately separate from the production assembly and is not an application
entry point.

This does not claim NativeAOT support for compiler, loader, editor or renderer
projects that produce or consume diagnostics. Those projects must be audited
independently.

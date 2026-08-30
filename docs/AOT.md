# NativeAOT support

`Delta.Diagnostics.Contract` is a value-only contract library and is marked
`<IsAotCompatible>true</IsAotCompatible>`. It has no reflection, dynamic code,
assembly loading, serializers or native resources. Its records, enums, `Guid`
and nullable value fields are ordinary statically reachable values; no runtime
metadata discovery is required.

This means a NativeAOT executable can reference the contract without adding
root descriptors or runtime code-generation hooks. A consuming executable is
still responsible for its own AOT/trimming configuration and for any other
dependencies it brings into the process.

`probes/DeltaDiagnostics.AotSmoke` is the bounded executable proof. It creates
and reads every public contract shape and is published with `PublishAot=true`
by `eng/aot-smoke.sh`, then executes the native binary. The probe is
deliberately separate from the contract assembly and is not a production
runtime entry point.

This does not claim NativeAOT support for compiler, loader, editor or renderer
projects that produce or consume diagnostics. Those projects must be audited
independently.

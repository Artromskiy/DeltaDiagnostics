# DeltaDiagnostics workflow

## Benchmark parameter policy

BenchmarkDotNet attributes may describe benchmark methods, categories and
lifecycle hooks, but they must not define workload or run parameters. Do not add
`[Params]`, `[ParamsSource]`, `[Arguments]`, `[ArgumentsSource]` or equivalent
parameter attributes. Parse every workload/configuration value from application
command-line arguments (or the invoking script) before BenchmarkDotNet starts,
and pass the resulting values into the benchmark runner. Keep BDN runner
switches such as `--filter` and `--job` separate from workload input. Existing
parameter attributes are migration debt: do not add new uses and replace them
when that benchmark is next modified.


## Repository layout gate

The repository must follow the shared first-party layout documented in the
Furnace project standard. Before restore/build or a structural handoff, run:

```bash
./eng/check-layout.sh
```

The gate checks the mandatory top-level directories, rejects unexpected
tracked top-level folders, requires src/DeltaDiagnostics/ as the primary source
project, and requires source siblings to use the src/DeltaDiagnostics.<Area>/ form.
samples/ contains runnable examples; probes/ contains bounded
headless/compiler/contract checks. Empty mandatory domains stay tracked with
.gitkeep.

Run the bounded checks from this repository root:

```bash
dotnet restore DeltaDiagnostics.slnx -p:NuGetAudit=false
dotnet build DeltaDiagnostics.slnx -c Release --no-restore \
  --disable-build-servers -m:1 /p:UseSharedCompilation=false -v:minimal
git diff --check
```

## NativeAOT smoke

The contract is AOT-compatible, but normal builds do not invoke the native
toolchain. Run the bounded publish probe when changing contract declarations
or project metadata:

```bash
./eng/aot-smoke.sh
```

Set `AOT_SMOKE_RID` to publish for another installed runtime identifier. The
probe writes its temporary native output outside the repository.

The contract has no test project or external package dependencies. Keep public
API changes deliberate and update the root contract registry when ownership or
direct consumers change.

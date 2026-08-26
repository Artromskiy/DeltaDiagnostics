# DeltaDiagnostics workflow

Run the bounded checks from this repository root:

```bash
dotnet restore DeltaDiagnostics.slnx
dotnet build DeltaDiagnostics.slnx -c Release --no-restore \
  --disable-build-servers -m:1 /p:UseSharedCompilation=false -v:minimal
git diff --check
```

The contract has no test project or external package dependencies. Keep public
API changes deliberate and update the root contract registry when ownership or
direct consumers change.

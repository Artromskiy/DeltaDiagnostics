#!/usr/bin/env bash
set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
rid="${AOT_SMOKE_RID:-osx-arm64}"
output_root="$(mktemp -d "${TMPDIR:-/tmp}/delta-diagnostics-aot.XXXXXX")"
trap 'rm -rf "$output_root"' EXIT

dotnet publish \
  "$repo_root/probes/DeltaDiagnostics.AotSmoke/DeltaDiagnostics.AotSmoke.csproj" \
  -c Release \
  -r "$rid" \
  --self-contained true \
  -p:AotSmoke=true \
  -p:NuGetAudit=false \
  --disable-build-servers \
  -m:1 \
  /p:UseSharedCompilation=false \
  -v:minimal \
  --output "$output_root"

binary="$output_root/DeltaDiagnostics.AotSmoke"
if [[ ! -x "$binary" ]]; then
  printf 'aot-smoke: published executable was not found: %s\n' "$binary" >&2
  exit 1
fi

"$binary"
printf 'aot-smoke: %s published and executed successfully\n' "$rid"

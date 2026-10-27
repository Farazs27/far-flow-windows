# Independent repository and delivery structure

The Windows edition uses its own repository `Farazs27/far-flow-windows` and sibling development folder `far-flow-windows`. It does not share a Git history, app identity, build outputs, release stream, signing material or user database with the Mac edition.

## Actual contents of this planning stage

```text
README.md
LICENSE
THIRD_PARTY_NOTICES.md
brand/
  assets/                 approved F, icon PNG and travertine artwork
  fonts/                  unmodified Zalando Sans plus OFL notice
research/
  00-brief.md
  10-findings-{1..4}.md    independent research/source audit
  20-validation-{1..4}.md  cross-assigned adversarial checks
  25-arbitration.md
  26-plan-review.md
  30-final-report.md
  *-evidence.json          read-only public artifact metadata
planning/
  model-candidates.json    research candidates, NOT an installer manifest
  dependency-candidates.json
  backlog.csv
  acceptance-matrix.csv
  evidence-status.json
  sources.json
  asset-provenance.json
  repository-layout.txt
scripts/
  validate_planning.py
.github/
  workflows/planning-checks.yml
  ISSUE_TEMPLATE/planning-feedback.md
docs/
  IMPLEMENTATION-PLAN.md
  ARCHITECTURE.md
  COMPATIBILITY-AND-MODELS.md
  MAC-TO-WINDOWS-AUDIT.md
  VALIDATION-AND-RELEASE.md
  PRIVACY-AND-SECURITY.md
  REPOSITORY-STRUCTURE.md
  decisions/
```

There is intentionally no `.sln`, `.csproj`, executable, native DLL, installer, model weight, Windows screenshot or fake success badge. A planning document check is not a Windows application build.

## Proposed implementation layout after approval

```text
src/
  FarFlow.Windows/             native WinUI shell, Pages/ViewModels/Styles/Assets
  FarFlow.Core/                portable rules, domain/session/schema
  FarFlow.WindowsIntegration/  Win32/COM/WASAPI/hook/UIA/media/window adapters
  FarFlow.ModelHost/           owned local inference worker, IPC/backends
  FarFlow.Infrastructure/      SQLite, backups, manifests, diagnostics
native/                       reviewed wrapper/build definitions where needed
models/manifests/             approved file metadata; weights remain external
tests/
  FarFlow.Core.Tests/
  FarFlow.Windows.Tests/
  FarFlow.ModelHost.Tests/
  FarFlow.IntegrationTests/
benchmarks/                   harness + invented/reviewed public-safe text; private audio excluded
packaging/                    primary signed package/update configuration
scripts/                      Windows setup/build/test/package/verification entry points
artifacts/                    ignored local outputs
```

Generate the first app from a verified Windows WinUI template, then build and launch it before layering features. Lock exact runtime/SDK/native/package versions and record build-host inventory. Do not generate a speculative XAML scaffold on macOS and call it verified.

## CI and publication policy

Current CI validates Markdown links, JSON/CSV metadata, required documents and brand hashes. Implementation adds an explicitly selected Windows runner/toolchain, restore/build/test/package checks and read-only PR permissions. Real desktop/audio/GPU tests use an owned Windows test host or documented manual runs; secrets and production signing never run on untrusted fork PRs.

Publish source and documentation separately from signed Windows release assets. Version models/runtime packs independently of app releases with manifest compatibility ranges. Keep a first supported packaging/update path before creating additional channels. Updates must preserve compatible data and never run while capture/insertion is active without a deliberate deferral/shutdown policy.

All rights reserved carries forward from the Mac repository. Dependencies, fonts and separately obtained weights retain their own terms. Do not publish internal Mac history, private research captures, user data, certificate stores or model caches as part of the new repository.

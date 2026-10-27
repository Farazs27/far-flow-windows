<p align="center"><img src="brand/assets/far-flow-icon.png" width="96" alt="Far Flow flowing F icon"></p>
<h1 align="center">Far Flow for Windows</h1>
<p align="center"><strong>Your voice. On your PC.</strong><br>A separate Windows edition of Far Flow, planned for private English and Dutch dictation.</p>

![Far Flow architectural brand artwork](brand/assets/travertine-banner.png)

**Status: research and implementation plan. A Windows application has not been built or certified yet.** This repository is independent of the [Mac edition](https://github.com/Farazs27/far-flow). The artwork above is a brand asset, not a Windows app screenshot.

The proposed product preserves local English/Dutch and mixed dictation, hold-to-talk and double-tap hands-free, insertion at your cursor, a compact movable waveform bar, dictionary/snippets, local history, optional local writing, and close-to-quit model lifetime.

## Read the plan

- [Implementation plan and approval decisions](docs/IMPLEMENTATION-PLAN.md)
- [Validated technical research](research/30-final-report.md)
- [Windows architecture](docs/ARCHITECTURE.md)
- [Hardware compatibility and model selection](docs/COMPATIBILITY-AND-MODELS.md)
- [Mac-to-Windows migration audit](docs/MAC-TO-WINDOWS-AUDIT.md)
- [Acceptance tests and release gates](docs/VALIDATION-AND-RELEASE.md)
- [Privacy and security design](docs/PRIVACY-AND-SECURITY.md)
- [Independent repository structure](docs/REPOSITORY-STRUCTURE.md)

## Proposed baseline

| Area | Proposed direction, pending Windows feasibility gates |
| --- | --- |
| App | C# / WinUI 3 / supported .NET LTS |
| First architecture | Windows 11 x64 on Intel/AMD; CPU-only operation required |
| GPU | Optional NVIDIA acceleration, including an RTX 3070-class validation tier |
| Speech | Benchmark Parakeet v3 int8 ONNX against multilingual Whisper alternatives |
| Cleanup | Fast local rules by default; optional separately downloaded GGUF writing model |
| Trigger | Configurable ordinary key; F8 is the proposed first-run default, rather than a universal Fn promise |
| Installer | Proposed signed full-trust MSIX; deployment spike before locking the channel |
| Data | Local storage, explicit model downloads, no required cloud inference |

These are design decisions and test targets, not measured Windows capabilities. An MSI gaming PC and an NVIDIA GPU are hardware examples, not a guarantee that every CPU, driver or memory configuration will perform identically.

## What exists today

This repository contains research dossiers, independent cross-validation, architecture/compatibility decisions, implementation phases, a release test matrix, approved reusable branding and documentation checks. There is no application scaffold, executable, installer, model weight bundle, or claimed Windows benchmark result.

All original Far Flow code, documentation and branding are **all rights reserved**; see [LICENSE](LICENSE). Included fonts and evaluated dependencies retain their own terms; see [third-party notices](THIRD_PARTY_NOTICES.md).

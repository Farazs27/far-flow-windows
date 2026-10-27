# Far Flow for Windows — research brief
Date: 27 October 2026
Depth: deep technical investigation with independent validation.

## Goal and decision
Prepare a separate Windows product/repository and an approval-ready implementation plan for native, fully local English/Dutch dictation with Far Flow’s existing brand and feature behavior. Decide the Windows architecture, inference/model strategy, OS/hardware support, packaging, release gates and staged implementation before application implementation.

## Scope and constraints
- New sibling folder: `far-flow-windows`; new independent Git history and GitHub repository `Farazs27/far-flow-windows`.
- Carry forward the public repository policy and all-rights-reserved notice; third-party licenses remain separate.
- Keep the Mac app/repository unchanged by this planning work.
- User requests broad PC compatibility, including MSI / RTX 3070-class machines; no exact test PC, RAM or GPU configuration has been supplied. Treat that as a hardware class, not proof of available test hardware.
- English, Dutch and mixed-language dictation; optional local writing; no mandatory cloud inference, login, subscription, NVIDIA GPU or NPU.
- Existing brand: flowing F, ivory/charcoal, olive, Zalando Sans, travertine artwork; Windows-native interactions and accessibility.
- Preserve hold-to-talk, double-tap hands-free, waveform-only movable overlay, cursor insertion, retained history/copy, dictionary, snippets, styles, reviewed transforms, scratchpad, insights, model management, explicit media pause ownership, subtle cues, and close-to-quit model lifetime.
- Do not assume the Mac Fn key, Core ML, MLX, AppKit, Accessibility APIs, media bridge or signing/installer can be copied to Windows.
- This host is macOS; Windows SDK/.NET/WinGet/Visual Studio and an attached Windows test device are not established. Research/docs/repository setup can complete here; a working Windows build and hardware validation require Windows evidence.
- The current Mac baseline is version 0.2.1 / public commit ccb628adbeea97ed898c35f1f83b21188815c32b, with 50 Mac unit tests and user-confirmed physical Fn operation. These are not Windows test results.

## Sub-questions
1. Inference: Which maintained local ASR and optional writing engines/models can provide English/Dutch/mixed speech across CPU-only Intel/AMD PCs and NVIDIA RTX 3070-class systems? Establish model language, license, CPU/GPU provider/artifact support and benchmark unknowns; compare credible fallback choices.
2. Platform integration: What must replace audio capture, Fn gestures, cursor insertion, media control and the nonactivating movable bar? Identify API limits, elevated/secure applications, clipboard behavior, device clock changes and lifecycle failure modes.
3. Native shell, lifecycle and delivery: Which Windows UI/.NET stack, stable toolchain, packaging, signing/update route and Windows support baseline are defensible? Separate Windows x64 and ARM64, supported OS releases, CI compilation versus interactive hardware testing, and release certification.
4. Migration and acceptance (primary agent): Audit the actual Mac code/behavior and define reusable specifications/assets, rewrites, data interoperability, isolation boundaries, product parity, performance targets, threat/privacy boundaries, test matrices, phases, risks and approval decisions.

## Research method
Prefer official Microsoft documentation, upstream repositories/releases, model cards and licenses. Each researcher records checkable claims, exact URLs, dates and gaps. Independently cross-assign validation; do not confirm a finding with the same source that introduced it. Preserve contradictions instead of converting untested performance assumptions into facts.

## Deliverables
- Intermediate researcher dossiers, validation reports and arbitration notes.
- A confidence-rated research report with dated primary sources and explicit unknowns.
- Detailed implementation plan, architecture decisions, compatibility/model decision matrix, parity audit, acceptance/release gates, risks, backlog and independent repository structure.
- A new public planning-stage repository; no Windows executable or Windows screenshots claimed before actual Windows build/run verification.

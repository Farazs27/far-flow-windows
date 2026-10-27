# Far Flow for Windows — implementation plan

Prepared 27 October 2026. **Phase 1 approved and implementation started.** This is the staged master plan. See [current execution evidence](PHASE-1-STATUS.md) for the preview build and remaining stop/go gates.

## 1. Product decision

Build an independent native Windows edition with the current Far Flow identity and the same useful dictation workflow. Use C# and WinUI 3 for the desktop shell, a platform-neutral .NET core for product rules, dedicated Windows adapters for OS integration, and an owned local model worker. Begin with supported Windows 11 x64 PCs, including CPU-only Intel/AMD machines and an optional NVIDIA tier. Do not require a subscription, cloud inference, an NPU, Python, or an NVIDIA GPU for baseline dictation.

The Windows app is a product port, not a recompile of the Xcode project. Preserve specifications, approved assets, data concepts and regression examples. Replace SwiftUI/AppKit, Core Audio, Accessibility, Core ML/MLX Swift bindings and Mac media/installer code. The [migration audit](MAC-TO-WINDOWS-AUDIT.md) identifies the boundary for each feature. Research facts and confidence appear in [the validated report](../research/30-final-report.md); recommendations and numerical targets here are engineering proposals.

## 2. Compatibility policy

First release: Windows 11 x64 on Microsoft-serviced consumer releases at release time. As of this research, prioritize 25H2 and 26H2; check edition-specific lifecycle again when shipping. Do not confuse a framework's Windows 10 runtime floor with a product support commitment. Windows 10, native Windows ARM64, Windows Server, Wine and secure desktops are outside the first release claim. ARM64 is a separate native-dependency/test lane, not a checked box based on x64 emulation.

Proposed CPU floor: four logical cores, x64/AVX2, 8 GB RAM for dictation/Light cleanup, with 16 GB recommended. These are provisional product requirements to validate, not asserted vendor-wide minima. An 8 GB machine must pass memory and latency gates before it is advertised as supported; otherwise raise the minimum or offer a proven lighter model. RTX 3070-class systems are a GPU test tier, with actual adapter memory/driver and laptop/desktop variant recorded. AMD/Intel graphics must not prevent the CPU path from working. [Detailed matrix](COMPATIBILITY-AND-MODELS.md).

## 3. Model selection policy

The leading speech candidate is multilingual **Parakeet TDT 0.6B v3 int8 ONNX through sherpa-onnx**. V3 documents Dutch; English-only v2 is not a substitute. Its reviewed int8 core artifact set is approximately 670 MB, not the Mac Core ML pack. Benchmark it against multilingual Whisper small/quantized on CPU and Whisper large-v3-turbo/quantized on the NVIDIA tier. Existing Mac results do not decide the Windows winner.

Keep standard Light cleanup deterministic and fast. Deeper cleanup, custom styles and transforms use a separately installed local writer, preferably a vetted GGUF conversion of the same **Qwen3-4B-Instruct-2507** checkpoint as the Mac product if provenance, license and performance pass. A generic Qwen3-4B hybrid-thinking checkpoint is a different candidate; never silently swap it or assume equivalent output. Do not select an 8B writer as the RTX 3070 default based only on file size.

Choose the default after real English/Dutch/mixed-voice benchmarks on the minimum CPU tier. GPU operation must report the effective provider during testing: requesting CUDA/DirectML is not proof of GPU execution. CPU fallback must remain functional and visible. DirectML/Vulkan, Qwen3-ASR and native ARM64 are research/extension lanes until their exact graph/build is tested. [Decision procedure](COMPATIBILITY-AND-MODELS.md).

## 4. User workflow

1. Launch the signed native Windows app. Setup explains microphone privacy, local models and retention.
2. Detect the chosen trigger. **F8** is a proposed configurable default. Hold it to record; release to insert. Double-tap it for hands-free, then press once to stop. Fn is offered only if that specific keyboard exposes usable events; it is not a universal Windows feature. Avoid an AltGr-conflicting default for Dutch layouts and avoid Windows-reserved chords.
3. Explicitly choose/download/import an approved speech pack. Warm the recognizer without leaving the microphone active. The ready dot means availability, not recording.
4. Dictate in English, Dutch or mixed speech. The nonactivating bar expands to a waveform only; no provisional words or timer are added.
5. At release/stop, freeze the intended target and caret/selection where readable. Revalidate before delivery. A changed, protected, elevated or unverifiable target yields retained text/Copy rather than an unsafe or falsely reported insertion.
6. History remains searchable/copyable according to retention. Dictionary, exact snippets, styles, transforms, scratchpad and insights follow the same local-data rules.
7. Closing the main window quits capture, hooks, overlays and owned model workers. Minimize keeps the app available. No implicit always-running model service is installed.

The Windows interface uses the flowing F, ivory/charcoal surfaces, olive state and Zalando Sans. Compose native WinUI controls and theme resources, retain standard Windows caption buttons/focus behavior, and override custom colors in High Contrast. Produce actual Windows screenshots only after a real build; the Mac screenshot and branded artwork are references, not Windows evidence.

## 5. Architecture boundaries

- **Desktop UI:** navigation, onboarding, preferences, history, editing and accessible status.
- **Core:** session state, dictionary/snippet/cleanup rules, protected edits, retention, schema and deterministic tests; no HWND/audio/runtime dependency.
- **Windows integration:** WASAPI, hook/message loop, foreground/UIA inspection, Unicode/paste delivery, media sessions, monitor/DPI placement and lifecycle.
- **Model worker:** pinned native model/runtime bundle, ASR and optional writer, cancellation/session IDs, bounded local IPC. It owns no global keyboard hook and no clipboard access.
- **Model manager:** explicit install/verify/import/remove, manifest paths/sizes/hashes, staging and activation; no downloads during transcription.

Create the worker in a kill-on-close Job Object at process creation, with explicit handle inheritance. Do not rely on a late Process.Start→AssignProcessToJobObject sequence or let the worker inherit the job handle. Job Objects manage lifetime/crash containment; they are not a security sandbox against malicious model code. [Architecture and contracts](ARCHITECTURE.md).

## 6. Implementation phases and gates

| Phase | Work and concrete deliverables | Gate to advance |
| --- | --- | --- |
| 0 — Research and approval | This plan, separate repository, source audit, independently validated dossiers, risk/compatibility decisions | Owner approves scope and first Windows test host is identified |
| 1 — Windows feasibility and model bake-off | Verified WinUI template/toolchain; real responsive native window; CPU ASR English/Dutch/mixed benchmark; minimal capture→recognition→Notepad path; packaged media/permission spike; insertion and nonactivating bar prototype | Clean Windows build/launch plus evidence that one complete local path works on CPU; select model/packaging by measurements |
| 2 — Reliable dictation engine | WASAPI native formats, route recovery, short/silent audio, timestamped hold/double-tap, cancellation, session isolation, owned worker lifetime and subtle original cues | 200 capture cycles without process exits, stuck microphone, duplicate sessions or orphan workers; format/lock/suspend tests pass |
| 3 — Cursor delivery and desktop integration | Target at stop; bounded UIA access, Unicode and deliberate clipboard fallback; explicit result confidence; waveform bar/DPI/drag; public media pause ownership | App/editor matrix passes, wrong-target/password tests insert nothing, terminal multiline is review/copy only, media resumes only the owned session |
| 4 — Full local workspace | Dictionary, snippets, styles, writer/transforms, scratchpad, history, insights, model manager, retention and reviewed Mac JSON import; full adaptive branded UI | Golden rules/schema fixtures pass; no silent translation/protected-span corruption in accepted cases; network-denied full workflow passes |
| 5 — Packaging and performance | Reproducible x64 package, CPU/NVIDIA profiles, app/taskbar/installer icons, signing/update/rollback, accessibility, memory/latency and install checks | Fresh standard-user PC installation and supported-hardware matrix pass; only measured claims go in README/release |
| 6 — Replacement pilot and release | Seven-day real-use pilot, reviewed defects, signed release assets/checksums/SBOM and genuine Windows screenshots | No open release-blocking failures; owner accepts documented compatibility limits and release artifacts |

Phase 1 is a genuine stop/go gate. If the chosen framework/provider/package combination fails, record the failure and change the affected decision before implementing the whole workspace. WPF is the native fallback to evaluate if WinUI interop/packaging materially blocks a proven requirement; a website wrapper is not the automatic fallback.

## 7. Definition of fully working

A completed source tree or successful CI compile is insufficient. The release needs a real responsive Windows app, offline speech on a CPU-only reference PC, measured GPU behavior on a reference NVIDIA PC, correct cursor delivery in the supported editor matrix, tested setup/update/uninstall, privacy/cancellation/exit invariants, and actual personal-voice usage.

The baseline guarantee is for tested standard-user desktop apps. Elevated apps, password fields, UAC/lock screens, protected/anti-cheat contexts, arbitrary remote sessions and every possible editor cannot honestly be promised as universal insertion targets. Recovery/Copy is part of correct operation. Do not silently type into a different field, overwrite a whole document through ValuePattern, or label an unverified paste as inserted.

Use the [acceptance matrix](VALIDATION-AND-RELEASE.md) as the release contract. Initial performance/accuracy numbers are proposed gates, not achieved benchmarks.

## 8. Repo, data and release separation

Repository: `Farazs27/far-flow-windows`; local folder: `far-flow-windows`. It has its own commits, app identity, solutions, dependency locks, CI, issues, versioning, releases and installer. No Mac build outputs, signing credentials, private history, recordings, model weights, local research captures or dependency caches are copied.

Reuse approved brand sources with provenance and font notices. Carry an explicit all-rights-reserved license. Windows code/model notices and SBOM describe actual shipped components, not the old Mac dependency list. For Mac full-data migration, use deliberate schema-1 full JSON backups with ISO 8601 dates; retain a separately validated dictionary-only JSON import/export path. Reset/remap Mac-only microphone IDs, Fn preferences and bundle-ID app styles. Do not copy a live SQLite database.

Primary packaging proposal: signed **full-trust MSIX**, with self-contained dependencies where practical and model files separate from the signed install directory. Validate its standard-user installation and actual capability behavior in Phase 1. An unpackaged per-user installer is the explicit alternative if evidence justifies it. Do not build two update systems before one delivery path works. Do not promise a universal one-file EXE: current Microsoft documentation contains conflicting single-file guidance and self-extraction adds a separate deployment/security/performance test surface.

## 9. Critical risks and mitigations

| Risk | Planned mitigation / decision owner |
| --- | --- |
| Dutch quality differs from Mac | Reviewed bilingual/mixed corpus and model bake-off before default selection |
| GPU pack silently uses CPU or requires missing DLLs | Pin native ABI/provider/runtime set, inspect effective provider, test with network disabled and no developer tools |
| Audio clock/Bluetooth route changes | Endpoint native format, explicit conversion contract, notification ownership and generation-scoped buffers |
| Hook/UIA delays freeze input | Dedicated fast hook thread; no inference/disk/IPC in callback; bounded off-UI-thread target inspection and isolated helper if providers can hang |
| Focus race or unsafe paste | Snapshot at stop, revalidate, no ambiguous retry, retained text/Copy, explicit insertion confidence |
| Clipboard data reaches Windows history/cloud | Avoid clipboard by default; mark temporary data excluded from history/sync, preserve original privacy markers during restoration and later user copies; test rather than promise secrecy from other clipboard readers |
| Worker outlives app | Creation-time Job Object assignment, noninherited sole owner handle, crash/kill/shutdown tests |
| Untrusted conversion/model | Approved upstream provenance, immutable revision and hashes; no arbitrary code/model imports; restricted load paths |
| Windows toolchain/runner mismatches | Exact selected SDK/runtime/runner inventory, independent Windows build+launch, clean-machine installer test |
| Signing availability/cost | Resolve publisher identity/eligible signing route before distribution; no signing purchase, credential upload or trust changes during planning |

## 10. Approval packet and estimates

Approve the **architecture and staged implementation**, not an unsupported claim of compatibility with every PC. Decisions proposed now: C#/WinUI, Windows 11 x64 first, CPU baseline plus optional NVIDIA, Parakeet-v3/Whisper bake-off, deterministic Light default, optional same-checkpoint 4B writer, configurable F8 gesture, brand continuity, close-to-quit, separate public reserved-rights repository and evidence-gated signed packaging.

Still needed for execution: a Windows interactive development/test host and the ability to test a CPU-only lane plus an RTX 3070-class lane. The existing Mac can support research, portable rule work and repository administration; it cannot certify Windows UI/audio/hooks or NVIDIA behavior. No VM, cloud PC, Windows license, signing subscription or hardware purchase is provisioned by this plan.

Planning estimate, not a delivery promise: roughly **25–45 focused engineering days plus the seven-day pilot**, assuming Windows hosts and signing access are available. Phase 1 should narrow that estimate substantially; provider/driver, insertion and packaging findings can change it. Approval can authorize Phase 1 alone, with its measurements reviewed before committing to the remaining phases.

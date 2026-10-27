# Far Flow for Windows — validated technical research
Date: 27 October 2026 | Depth: deep, cross-assigned validation | Findings reviewed: 45

Legend: 🟢 Confirmed within scope · 🟡 Partial, corrected/narrowed · 🔴 Contested · ⚪ Unverified runtime result.

## Executive summary

A separate native Windows edition is a defensible engineering direction, but it requires new Windows integrations and inference artifacts rather than an Xcode rebuild. The research produced 45 findings; independent cross-validation confirmed 32 within their stated scope and corrected or narrowed 13. The strongest proposed baseline is C#/WinUI with CPU-only Windows 11 x64 operation and a separately measured NVIDIA acceleration tier. Parakeet v3 int8 is a leading English/Dutch ASR candidate, while Whisper provides an important comparison/fallback; the exact Instruct-2507 writer identity must be preserved if it is converted to GGUF. Fn portability, safe cursor delivery, Bluetooth/device formats, worker ownership and real installer behavior are release-critical differences. **No Windows executable, launch, hardware benchmark or personal-voice certification has been produced in this macOS planning session.** The detailed plan is ready for approval; Windows implementation/verification is the next phase.

## Decision implications

Choose an ordinary configurable Windows key and preserve hold/double-tap behavior instead of promising universal Fn support. Use Windows audio/foreground/UIA/media/window APIs with a fail-closed delivery contract. Keep fast deterministic cleanup as the default and load the optional writer only when needed. Select model/provider defaults by a reviewed CPU/NVIDIA bake-off, not a Mac timing table, GPU name or file-size estimate. Use creation-time owned worker lifetime and explicit close/minimize behavior. Keep the new repository/data/build/release history independent from the Mac product.

These are engineering recommendations derived from the corrected findings, not shipped behavior. Numerical compatibility, accuracy and speed goals in the companion plan are proposed acceptance gates.

## Key findings, after validation

The compact ledger below carries every reviewed atomic finding. Each evidence link opens the original dated/source-linked dossier and the independently produced validation, including qualifications and contradictions.

### Local inference and artifact choices

| ID | Confidence | Corrected conclusion | Evidence |
| --- | --- | --- | --- |
| F1.1 | 🟢 | Parakeet v3 advertises English and Dutch; the int8 Windows conversion still needs personal-voice and mixed-speech evaluation. | [Research](10-findings-1.md) · [Validation](20-validation-1.md) |
| F1.2 | 🟢 | Parakeet v2 is an English-only candidate and cannot establish Dutch support. | [Research](10-findings-1.md) · [Validation](20-validation-1.md) |
| F1.3 | 🟢 | The inspected Parakeet v3 int8 payload has encoder, decoder, joiner and tokens totaling 670,478,772 bytes; configuration and derivative notices remain separate gates. | [Research](10-findings-1.md) · [Validation](20-validation-1.md) |
| F1.4 | 🟢 | Managed sherpa C# bindings and Windows native packages exist; ABI/RID/provider packaging must be tested in Far Flow. | [Research](10-findings-1.md) · [Validation](20-validation-1.md) |
| F1.5 | 🟡 | Provider requests may fall back or partition onto CPU; validate actual initialization, node placement, dtype correctness and speed, rather than a provider string. | [Research](10-findings-1.md) · [Validation](20-validation-1.md) |
| F1.6 | 🟢 | Multilingual Whisper small/turbo and quantized artifacts exist; .en variants are a different language scope. | [Research](10-findings-1.md) · [Validation](20-validation-1.md) |
| F1.7 | 🟢 | whisper.cpp supports Windows CPU/CUDA/Vulkan build paths; tagged source and binary/nightly distribution are distinct evidence. | [Research](10-findings-1.md) · [Validation](20-validation-1.md) |
| F1.8 | 🟢 | Compiled Core ML artifacts are platform backend outputs, not Windows runtime inputs; source models can be evaluated in different conversions. | [Research](10-findings-1.md) · [Validation](20-validation-1.md) |
| F1.9 | 🟡 | Generic Qwen3-4B is not the Mac Instruct-2507 checkpoint. Vetted community GGUFs of the exact non-thinking checkpoint are separate candidates. | [Research](10-findings-1.md) · [Validation](20-validation-1.md) |
| F1.10 | 🟢 | Runtime memory includes weights, KV/context and compute buffers; disk bytes alone cannot prove RTX 3070 fit. | [Research](10-findings-1.md) · [Validation](20-validation-1.md) |
| F1.11 | 🟢 | Qwen3-ASR has documented Dutch coverage as an additional candidate, without a Windows export/provider/performance result here. | [Research](10-findings-1.md) · [Validation](20-validation-1.md) |
| F1.12 | 🟢 | Model files exercise native parsers. Hash identity and helper processes do not establish a malicious-model sandbox. | [Research](10-findings-1.md) · [Validation](20-validation-1.md) |

### Windows desktop integrations

| ID | Confidence | Corrected conclusion | Evidence |
| --- | --- | --- | --- |
| F2.1 | 🟢 | Fn is not a universal standalone Windows trigger; an ordinary detected/rebindable key is required for a broad PC product. | [Research](10-findings-2.md) · [Validation](20-validation-2.md) |
| F2.2 | 🟢 | RegisterHotKey provides press notification, not a global key-up contract for hold-to-talk. | [Research](10-findings-2.md) · [Validation](20-validation-2.md) |
| F2.3 | 🟡 | Slow low-level hooks can cease receiving events; return immediately and use timestamped edge events. Exact timeout details have narrower independent corroboration. | [Research](10-findings-2.md) · [Validation](20-validation-2.md) |
| F2.4 | 🟢 | WASAPI shared capture can use the endpoint mix format; ASR normalization remains an explicit contract, with application or engine conversion choices. | [Research](10-findings-2.md) · [Validation](20-validation-2.md) |
| F2.5 | 🟢 | Direct capture clients own endpoint/format-change handling; Bluetooth communications modes expand the rate/channel test matrix. | [Research](10-findings-2.md) · [Validation](20-validation-2.md) |
| F2.6 | 🟢 | ValuePattern sets a value, not an arbitrary caret insertion; TextPattern itself is read-only. Pattern coexistence is possible. | [Research](10-findings-2.md) · [Validation](20-validation-2.md) |
| F2.7 | 🟢 | Ordinary SendInput is constrained by integrity levels; secure/password detection and safe copy recovery are product responsibilities. | [Research](10-findings-2.md) · [Validation](20-validation-2.md) |
| F2.8 | 🟢 | Unicode input routes to the foreground thread, so target validation is not an atomic delivery transaction; clipboard/terminal recovery needs explicit policy. | [Research](10-findings-2.md) · [Validation](20-validation-2.md) |
| F2.9 | 🟡 | Nonactivating HWND styles need nonactivating API use, DPI/monitor handling and a WM_MOUSEACTIVATE hover guard. | [Research](10-findings-2.md) · [Validation](20-validation-2.md) |
| F2.10 | 🟢 | GSMTC exposes cooperating media sessions, not every audio producer; exact package/capability behavior remains a Windows spike. | [Research](10-findings-2.md) · [Validation](20-validation-2.md) |
| F2.11 | 🟢 | Media pause success is not a causal ownership token. Retain the exact session and abandon ambiguous resume ownership. | [Research](10-findings-2.md) · [Validation](20-validation-2.md) |
| F2.12 | 🟡 | Kill-on-last-job-handle-close supports owned lifetime; use creation-time job assignment and noninherited sole ownership to avoid the late-assignment orphan window. | [Research](10-findings-2.md) · [Validation](20-validation-2.md) |

### Native shell, toolchain and delivery

| ID | Confidence | Corrected conclusion | Evidence |
| --- | --- | --- | --- |
| F3.1 | 🟢 | As of the research date, .NET 10 is LTS; catalog metadata shows SDK 10.0.403 / runtime 10.0.14. | [Research](10-findings-3.md) · [Validation](20-validation-3.md) |
| F3.2 | 🟢 | As of the research date, the highest stable Windows App SDK package is 2.5.1, not a newer-dated servicing patch from an older major line. | [Research](10-findings-3.md) · [Validation](20-validation-3.md) |
| F3.3 | 🟢 | Microsoft recommends WinUI for new native Windows UI; task-specific framework tradeoffs still require a feasibility gate. | [Research](10-findings-3.md) · [Validation](20-validation-3.md) |
| F3.4 | 🟢 | Framework runtime floors and edition-specific Windows servicing are different from the product support/test policy. | [Research](10-findings-3.md) · [Validation](20-validation-3.md) |
| F3.5 | 🟢 | A normal full-trust WinUI MSIX is not the UWP AppContainer model; activation/manifest attributes matter. | [Research](10-findings-3.md) · [Validation](20-validation-3.md) |
| F3.6 | 🟡 | Both .NET and Windows App SDK runtime closure matter. Single-file guidance conflicts and self-extraction is not the baseline delivery promise. | [Research](10-findings-3.md) · [Validation](20-validation-3.md) |
| F3.7 | 🟢 | Microphone/media declarations and access depend on app model; actual signed deployment must be tested. | [Research](10-findings-3.md) · [Validation](20-validation-3.md) |
| F3.8 | 🟢 | Normal WinUI desktop backgrounding is not UWP suspension; close/minimize/worker lifetime remain app-owned behavior. | [Research](10-findings-3.md) · [Validation](20-validation-3.md) |
| F3.9 | 🟢 | Signing does not guarantee immediate SmartScreen reputation; actual publisher eligibility and certificate trust remain provisioning gates. | [Research](10-findings-3.md) · [Validation](20-validation-3.md) |
| F3.10 | 🟡 | App Installer supports managed updates, but prompt/block behavior varies with app model/launch source and must be tested. | [Research](10-findings-3.md) · [Validation](20-validation-3.md) |
| F3.11 | 🟢 | A pure native ARM64 process requires matching native binaries; emulation/ARM64EC and separate-worker architectures are distinct lanes. | [Research](10-findings-3.md) · [Validation](20-validation-3.md) |
| F3.12 | 🟡 | This macOS planning session has no Windows execution evidence. Actual runner images differ; select an explicit VS2026 image/toolchain instead of assuming windows-2025 contains it. | [Research](10-findings-3.md) · [Validation](20-validation-3.md) |

### Actual Mac migration boundary

| ID | Confidence | Corrected conclusion | Evidence |
| --- | --- | --- | --- |
| F4.1 | 🟢 | The audited public Mac tree targets macOS/arm64 and has no Windows project in that revision. | [Research](10-findings-4.md) · [Validation](20-validation-4.md) |
| F4.2 | 🟡 | The current app/artifacts are Mac-targeted and unvalidated on Windows; do not generalize that MLX/MLX Swift is universally Mac-only. | [Research](10-findings-4.md) · [Validation](20-validation-4.md) |
| F4.3 | 🟡 | The Mac gesture starts capture on first press; 220 ms classifies release/hold behavior and 350 ms is the configured second-tap window, not a delayed start. | [Research](10-findings-4.md) · [Validation](20-validation-4.md) |
| F4.4 | 🟡 | Full UserData backup JSON uses an explicit ISO-8601 path; dictionary-only JSON and internal SQLite serialization are different contracts. | [Research](10-findings-4.md) · [Validation](20-validation-4.md) |
| F4.5 | 🟡 | Portable rule concepts coexist with Apple language-ID/culture behavior; full Unicode/noncascading/mixed-language parity needs expanded fixtures. | [Research](10-findings-4.md) · [Validation](20-validation-4.md) |
| F4.6 | 🟢 | Mac bundle identifiers, integer audio device IDs and Fn preferences require explicit Windows mapping/reset policies. | [Research](10-findings-4.md) · [Validation](20-validation-4.md) |
| F4.7 | 🟢 | The current product intentionally quits on main-window close and distinguishes minimize; source paths are not universal forced-kill runtime proof. | [Research](10-findings-4.md) · [Validation](20-validation-4.md) |
| F4.8 | 🟡 | The Mac record reports 50 passing tests; definitions and synthetic results were corroborated, but the validator did not rerun XCTest. None is Windows evidence. | [Research](10-findings-4.md) · [Validation](20-validation-4.md) |
| F4.9 | 🟢 | Approved F/artwork/font sources are available with separate font terms; choose independently licensed Windows icon/sound assets. | [Research](10-findings-4.md) · [Validation](20-validation-4.md) |

## Contradictions and open questions

- 🟡 **GPU graphs/providers:** current documentation and native build artifacts do not establish that the selected int8 graph is fully accelerated or faster. Actual EP initialization/placement/dtype/output checks decide. [ORT provider architecture](https://onnxruntime.ai/docs/execution-providers/), [CUDA EP](https://onnxruntime.ai/docs/execution-providers/CUDA-ExecutionProvider.html).
- 🟡 **Single-file delivery:** Microsoft self-contained and dedicated unpackaged/single-file guidance differ; current .NET guidance qualifies all-content extraction as compatibility behavior. Keep a signed package/folder path as the proposed baseline and test the exact selected deployment. See F3.6 validation.
- 🟡 **Toolchain inventory:** a migration announcement is weaker than current image inventory. The inspected ordinary Windows2025 image contained VS2022/SDK10.0.402; the separate VS2026 image differs. [Current image manifests](https://github.com/actions/runner-images/tree/main/images/windows).
- 🟡 **Full backup versus dictionary-only JSON:** the Mac has both workflows; only the full backup follows the reviewed UserData/date/schema contract. Preserve both intentionally rather than dropping one through vague import wording.
- ⚪ **Final support floor, model winner, provider pack, language-ID adapter, writer context and signing route:** these depend on Windows measurements, artifact notice verification and actual publisher/test-host availability.

## What we could NOT verify

There is no Windows native build/run, real physical trigger test, microphone/device/Bluetooth test, editor insertion matrix, CPU/NVIDIA inference result, 3070 memory-fit result, signed clean-machine installation or seven-day pilot in this planning work. Exact user PC specs and a Windows interactive host are not supplied. No model weights were downloaded, so publisher LFS metadata was not relabeled as locally recomputed model hashes. Exact derivative notice completeness, future runtime vulnerabilities, package-specific GSMTC behavior, all editor/password providers and arbitrary protected/elevated insertion remain unverified. A detected GPU, listed package or framework minimum OS does not close those gaps.

## So what — recommended execution

Approve Phase 1 of the [implementation plan](../docs/IMPLEMENTATION-PLAN.md) first: establish a Windows host/toolchain, show a responsive native window and one real local capture→ASR→Notepad path, and benchmark the shortlisted speech engines on CPU. In parallel, prove the nonactivating bar, safe target delivery and chosen package/media behavior. Only after those gates should the rest of the full workspace be implemented and the GPU/writer profiles enabled. The final release contract requires real Windows evidence and a replacement pilot; it cannot be satisfied by adding C# files or relabeling Mac tests.

## Sources

The [complete dated source index](31-sources.md) and [machine-readable index](../planning/sources.json) cover the dossier and validation URLs. Source publication/update dates are recorded where exposed; access date is 27 October 2026, not a fabricated publication date. Important primary anchors include:

- [NVIDIA Parakeet v3 model card](https://huggingface.co/nvidia/parakeet-tdt-0.6b-v3) and [model-author technical report](https://arxiv.org/html/2509.14128v2) — model capability, not converted Windows performance.
- [sherpa-onnx source/releases](https://github.com/k2-fsa/sherpa-onnx), [whisper.cpp](https://github.com/ggml-org/whisper.cpp), [llama.cpp](https://github.com/ggml-org/llama.cpp) — engine/native artifact contracts and security surfaces.
- [Exact Qwen3-4B-Instruct-2507](https://huggingface.co/Qwen/Qwen3-4B-Instruct-2507) — exact checkpoint identity; community conversion metadata is separately recorded.
- [Microsoft keyboard guidance](https://learn.microsoft.com/en-us/windows/powertoys/keyboard-manager), [WASAPI capture](https://learn.microsoft.com/en-us/windows/win32/coreaudio/capturing-a-stream), [SendInput](https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-sendinput) — platform boundaries.
- [Clipboard formats/history/cloud controls](https://learn.microsoft.com/en-us/windows/win32/dataxchg/clipboard-formats) and [job-creation race analysis](https://devblogs.microsoft.com/oldnewthing/20230209-00/?p=107812) — independent corrections with direct design implications.
- [Windows App SDK downloads](https://learn.microsoft.com/en-us/windows/apps/windows-app-sdk/downloads), [.NET support policy](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core), [code-signing options](https://learn.microsoft.com/en-us/windows/apps/package-and-deploy/code-signing-options) — current catalog/lifecycle/distribution context.
- [Audited Mac baseline](https://github.com/Farazs27/far-flow/tree/ccb628adbeea97ed898c35f1f83b21188815c32b) — source behavior and original brand provenance, not Windows verification.

## Audit trail

[Brief](00-brief.md) · Research dossiers 1–4 · Independent validation 1–4 · [Arbitration](25-arbitration.md) · [Plan review](26-plan-review.md) · [Claim ledger](../planning/claim-ledger.json) · [Unrun Windows evidence status](../planning/evidence-status.json).

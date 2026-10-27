# Hardware compatibility and model decision

> Planning/release-contract snapshot. See [Phase 1 execution and current evidence](PHASE-1-STATUS.md) for the implemented preview and the tests actually run.

Status: proposed support/benchmark policy. **No Windows speed, accuracy, memory or GPU-fit result has been produced.** MSI is treated as a PC manufacturer; RTX 3070 is a GPU-class example, not an exact supplied device specification.

## Support tiers

| Tier | Proposed configuration | Planned support |
| --- | --- | --- |
| A — Baseline | Serviced Windows 11 x64, Intel/AMD x64 with AVX2, four logical cores, 8 GB RAM | CPU-only dictation + Light rules only if minimum-tier gates pass |
| B — Recommended | Same OS/ISA, modern CPU, 16 GB RAM | Full workspace; optional 4B local writing after latency/memory validation |
| C — NVIDIA | Tier B plus supported NVIDIA driver and measured dedicated VRAM; RTX 3070-class reference device | Optional acceleration; CPU path remains usable |
| D — Higher-memory GPU | Independently tested adapter/driver/VRAM | Larger writer/longer context only after explicit tests |
| Deferred | Native Windows ARM64, Windows 10, Server, Wine, unsupported OS releases, 32-bit x86 | Not a v1 compatibility claim; revisit in separate lanes |

The AVX2/8 GB proposal is an initial product floor, not a claim that every upstream runtime requires AVX2 or every 8 GB PC is fast enough. Probe CPU features before loading an ISA-specific native binary. Record runtime ABI, DLL set and effective provider. Do not compile a public CPU release using build-host-only native optimizations. A 4 GB machine or unknown GPU is not silently declared supported.

Support policy follows Windows edition/channel servicing at release time. Framework runtime support back to older Windows does not imply that Far Flow tests those systems. As of 27 October 2026, consumer 25H2/26H2 are priority lanes; 24H2 Home/Pro servicing ended while enterprise dates differ. Re-check Microsoft lifecycle before release. [F3.4]

## Candidate models

| Role | Candidate | Why evaluate | Main gate |
| --- | --- | --- | --- |
| First ASR candidate | NVIDIA Parakeet TDT 0.6B **v3**, sherpa int8 ONNX conversion | Documented English/Dutch coverage; separate encoder/decoder/joiner/tokens contract | Real bilingual/mixed accuracy, native CPU speed, exact preprocessing and notices |
| CPU comparison/fallback | Multilingual Whisper small, including vetted quantized GGML | Maintained Windows-capable CPU path and multilingual coverage | CPU latency, personal Dutch quality, quantization effects |
| NVIDIA comparison | Whisper large-v3-turbo / vetted quantized GGML | CUDA and Vulkan build paths exist upstream | Actual provider/build, memory and comparative latency |
| Optional writer | Same-base Qwen3-4B-Instruct-2507 community GGUF, typically Q4_K_M candidate | Maintains exact intended non-thinking checkpoint identity | Derivative provenance/license, template/tokenizer, output safety and memory |
| Additional research | Qwen3-ASR-0.6B | Official Dutch coverage and current sherpa integration surface | Separate artifact/provider/quality evaluation, not an automatic scope expansion |

Do not choose Parakeet v2 for Dutch, `.en` Whisper artifacts for bilingual use, or generic Qwen3-4B solely because the parameter count matches. Generic original Qwen3 hybrid-thinking and Instruct-2507 are different post-trained checkpoints. No Qwen-owned exact Instruct-2507 GGUF was established by this research; pinned community candidates are recorded in validation-1. [F1.1–F1.11]

The inspected Parakeet int8 model payload is **670,478,772 bytes** across four selected files, excluding runtime DLLs/archive/installer. This is publisher/listing metadata and arithmetic, not a locally downloaded checksum or peak RAM measurement. The FP32 export has a separate external weights file and must not be mistaken for the int8 bundle. The conversion uses a 128-feature configuration; copy its metadata rather than a generic ASR default.

Generic official Qwen3-4B Q4_K_M was listed at about 2.50 GB and 8B at about 5.03 GB, but those are *different checkpoint examples*, not byte counts for the preferred Instruct-2507 writer or a VRAM-fit result. Exact selected derivative bytes, revision, hashes and notices are locked only after the feasibility spike. Weights, KV cache, context, batch/compute buffers, ASR and desktop usage all contribute to runtime memory.

## Engine/provider matrix

| Engine lane | Required proof |
| --- | --- |
| sherpa / ONNX Runtime CPU | Matching C# wrapper and `win-x64` native package; correct model graph/config; no Python or network fetch on transcription |
| sherpa CUDA | CUDA-enabled wrapper/native/ORT/dependency combination, exact model dtype/op placement, real NVIDIA execution and improvement; normal CPU NuGet package is not a GPU proof |
| whisper.cpp CPU | Pinned source/native build and multilingual artifact; conservative x64 ISA settings and CPU-only test |
| whisper.cpp CUDA/Vulkan | Separate backend build, correct DLLs/driver/device selection and actual artifact correctness/performance |
| llama.cpp writer | Exact GGUF checkpoint/quant/template; CPU and optional GPU memory/latency; safe output and cancellation |
| DirectML | Deferred unless exact exported graph and runtime pass; current docs impose sequential/session constraints and upstream marks it sustained-engineering |

A requested provider, a provider list or a detected GPU is not proof of execution on it. Record initialization, node/subgraph placement where available, and end-to-end results. Silent CPU fallback cannot be presented as a GPU benchmark. Test CPU with the GPU genuinely disabled for that runtime; for the inspected llama.cpp version, GPU offload count alone is insufficient evidence.

## Benchmark method and selection rule

1. Pin candidate revision, conversion, filenames, byte counts, hashes, tokenizer/config and license provenance. Use only explicit verified downloads; no multi-GB weights belong in Git.
2. Prepare at least 200 reviewed natural clips: 80 English, 80 Dutch, 40 mixed, multiple speakers and quiet/noisy/microphone conditions; add silence/short-speech/number/name/negation cases separately. Use synthetic clips only as regression starters.
3. Run the identical corpus on a CPU-only minimum reference PC, recommended CPU PC and NVIDIA reference PC. Record exact CPU, RAM, GPU/VRAM, driver, OS, power/battery, thread settings, model precision/context and thermal state.
4. Measure cold start, warm inference real-time factor, key-up-to-verified-insert latency, cancellation, failures, process working set and GPU allocations. Report ASR WER separately from cleanup safety and insertion outcomes.
5. Select the CPU default by hard quality/privacy/reliability gates first, then latency/memory/download tradeoffs. If Parakeet fails a gate, choose a proven Whisper configuration or revise the support floor; do not keep an advertised model solely for continuity with Mac.
6. Enable a GPU profile only when it is correct and materially useful on the reference GPU. The 4B writer is optional/lazy; 8B is not the default for an unknown 3070 VRAM budget.

Proposed initial scoring after hard gates: 40% English/Dutch/mixed quality, 25% actual stop-to-insert time, 15% memory/stability, 10% packaging/offline portability, 10% download and maintenance burden. Scores are **not yet filled in**.

## Proposed performance gates — not achieved results

For warmed speech on reviewed clips up to 10 seconds, target p95 stop-to-verified-insert ≤3 seconds on the recommended CPU reference and ≤1.5 seconds on the NVIDIA reference. For ASR compute, initially target real-time factor ≤0.25 CPU / ≤0.10 GPU on those named references; longer utterances need separate reporting. First captured-buffer latency target is p95 ≤250 ms after the trigger on a ready app; do not count a fake waveform as audio capture.

Initial quality goals for the reviewed normal-condition set: raw WER ≤12% English, ≤15% Dutch, ≤20% mixed, plus no accepted protected-number/negation changes in the cleanup safety fixtures. These are product targets to review after the first corpus run, not model-card promises or statistical guarantees for every speaker.

Minimum-tier 8 GB acceptance, writer context/batch size, actual peak VRAM reserve, thermal/battery behavior and installer disk footprint remain measured decisions. A reference GPU should retain a defined memory reserve; it is not acceptable to claim a fit from GGUF disk size alone.
